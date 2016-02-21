using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Dnxt.DtoGeneration;
using JetBrains.Annotations;

namespace Dnxt.UnitTests
{
    public class SqlSchemaGenerator
    {
        [NotNull]
        private readonly Domain _domain;

        public SqlSchemaGenerator([NotNull] Domain domain)
        {
            if (domain == null) throw new ArgumentNullException(nameof(domain));
            _domain = domain;
            _writedEntities = new List<EntityModel>();
        }

        [NotNull]
        private readonly List<EntityModel> _writedEntities;

        public StringBuilder GetSchemaSql()
        {
            var sb = new StringBuilder();
            var entityModels = _domain.Entities.Values;
            foreach (var model in entityModels)
            {
                WriteIfNotExist(sb, model);
            }

            foreach (var model in entityModels)
            {
                foreach (var reference in model.References)
                {
                    WriteFks(sb, model, reference);
                }
            }

            return sb;
        }

        private void WriteIfNotExist([NotNull] StringBuilder sb, [NotNull] EntityModel model)
        {
            var exists = _writedEntities.FirstOrDefault(m => m == model);
            if (exists != null)
            {
                return;
            }

            WriteTable(sb, model);
            _writedEntities.Add(model);
        }

        private void WriteTable([NotNull] StringBuilder sb, [NotNull] EntityModel entity)
        {
            sb.AppendLine($"CREATE TABLE \"{entity.Name}\" (");

            var columns = entity.Properties.OrderBy(model => model.IsKey()).Select(GetColumnDefinition).ToList();
            sb.AppendLine(string.Join("," + Environment.NewLine, columns));

            WritePks(sb, entity);
            sb.AppendLine(");");
            sb.AppendLine();
            WriteIndexes(sb, entity);
        }

        private string GetColumnDefinition([NotNull] PropertyModel property)
        {
            var propType = property.Type;
            var isNullable = propType.IsClass && !property.IsNotNull();

            if (propType.IsNullable())
            {
                propType = propType.GetGenericArguments().First();
                isNullable = true;
            }

            var notNullStr = isNullable ? "" : " NOT NULL";

            var dataType = GetDataType(propType, property.IsKey(), property.Attributes);
            return ($"\t\"{property.Name}\" {dataType}{notNullStr}");
        }

        private void WriteFks([NotNull] StringBuilder sb, [NotNull] EntityModel entity, [NotNull] RefModel reference)
        {
            var refTable = reference.Entity;
            WriteIfNotExist(sb, refTable);

            var refKey = refTable.Properties.SingleOrDefault(model => model.IsKey());
            if (refKey == null) throw new InvalidOperationException($"Entity '{refTable}' doesn't contains any key property.");

            sb.AppendLine($"ALTER TABLE \"{entity.Name}\"");
            sb.Append("ADD COLUMN ").Append(GetColumnDefinition(refKey)).AppendLine(";");


            sb.AppendLine($"ALTER TABLE \"{entity.Name}\"");
            sb.AppendLine($"    ADD CONSTRAINT \"{GetConstrName(refTable, refKey, "FK")}\" FOREIGN KEY (\"{reference.Name}\") REFERENCES \"{reference.Entity.Name}\"(\"{refKey}\")");
            sb.AppendLine("     ON UPDATE RESTRICT ON DELETE CASCADE;");
            sb.AppendLine($"    CREATE INDEX \"fki_{refTable.Name}_{refKey.Name}\" ON \"{refTable.Name}\"(\"{refKey.Name}\");");

            var keyProp = refTable.Properties.First(model => model.IsKey());
            sb.Append($" references \"{refTable.Name}\"(\"{keyProp.Name}\")");
        }

        private string GetDataType([NotNull] Type propType, bool isKey, [NotNull] IEnumerable<object> attributes)
        {
            if (isKey && propType == typeof(int))
            {
                return "Serial";
            }

            const string smallInt = "SmallInt";

            if (propType.IsEnum)
            {
                return smallInt;
            }

            var dict = new Dictionary<Type, Func<string>>()
            {
                {typeof(bool),  () => "Boolean"},
                {typeof(short),  () => smallInt},
                {typeof(int),  () => "integer"},
                {typeof(long),  () => "bigint"},
                {typeof(DateTime),  () => "TimeStamp"},
                {typeof(DateTimeOffset),  () => "timestamp with time zone"},
                {typeof(TimeSpan),  () => "TimeStamp"},
                {typeof(decimal),  () => "Decimal(10,4)"},
                {typeof(double),  () => "Decimal(19,4)"},
                {typeof(Guid),  () => "UUID"},
                {typeof(string), () =>
                    {
                        var maxLength = attributes.OfType<MaxLengthAttribute>().FirstOrDefault();

                        if (maxLength != null && maxLength.Length > 0)
                        {
                            return $"VarChar({maxLength.Length})";
                        }

                        return "VarChar";
                    }
                },
            };

            Func<string> func;
            if (dict.TryGetValue(propType, out func))
            {
                return func();
            }

            throw new NotSupportedException("Property Type: " + propType);
        }

        private string GetConstrName([NotNull] EntityModel table, [NotNull] PropertyModel property, string constType)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (property == null) throw new ArgumentNullException(nameof(property));

            return $"{table.Name}_{property.Name}_{constType}";
        }

        private void WriteIndexes([NotNull] StringBuilder sb, [NotNull] EntityModel tableModel)
        {
            if (tableModel == null) throw new ArgumentNullException(nameof(tableModel));

            foreach (var property in tableModel.Properties)
            {
                var indexAttribute = property.Attributes.OfType<IndexAttribute>()
                    .FirstOrDefault();

                var keyAttribute = property.Attributes.OfType<KeyAttribute>()
                    .FirstOrDefault();


                if (indexAttribute != null || keyAttribute != null)
                {
                    bool isUnique = (indexAttribute != null && indexAttribute.IsUnique) || keyAttribute != null;

                    var uniqueDef = isUnique ? "UNIQUE " : "";

                    var idxName = GetConstrName(tableModel, property, "Idx");
                    sb.AppendLine(string.Format("CREATE {2} INDEX \"{3}\" ON \"{0}\" USING btree (\"{1}\");", tableModel.Name, property.Name, uniqueDef, idxName));
                }
            }
        }

        private void WritePks([NotNull] StringBuilder sb, [NotNull] EntityModel table)
        {
            var models = table.Properties.Where(model => model.IsKey());
            foreach (var model in models)
            {
                var constrName = GetConstrName(table, model, "pkey");
                sb.AppendLine(string.Format("CONSTRAINT \"{1}\" PRIMARY KEY (\"{0}\")", model.Name, constrName));
            }
        }
    }
}