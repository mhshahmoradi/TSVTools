using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TSVTools
{
    public class TsvFile<T> where T : new()
    {
        private readonly List<T> _rows = new List<T>();

        public List<string> ColumnNames { get; }

        public int Count => _rows.Count;

        public TsvFile()
        {
            ColumnNames = typeof(T).GetProperties().Select(p => p.Name).ToList();
        }

        public TsvFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File '{filePath}' not found.");
            }

            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                throw new ArgumentException($"File '{filePath}' is empty.");
            }

            ColumnNames = new List<string>(lines[0].Split('\t'));

            for (int i = 1; i < lines.Length; i++)
            {
                var rowValues = lines[i].Split('\t');

                if (rowValues.Length != ColumnNames.Count)
                {
                    throw new ArgumentException(
                        $"Line {i} in file '{filePath}' does not have the same number of columns as the header.");
                }

                var row = new T();

                for (int j = 0; j < ColumnNames.Count; j++)
                {
                    var property = typeof(T).GetProperty(ColumnNames[j]);
                    var value = rowValues[j];
                    object typedValue = null;

                    if (property.PropertyType.IsEnum)
                    {
                        typedValue = Enum.Parse(property.PropertyType, value, true);
                    }
                    else
                    {
                        typedValue = Convert.ChangeType(value, property.PropertyType);
                    }

                    property.SetValue(row, typedValue);
                }

                _rows.Add(row);
            }
        }

        public void AddRow(T row)
        {
            _rows.Add(row);
        }

        public void DeleteRow(int index)
        {
            _rows.RemoveAt(index);
        }

        public void AppendFile(string filePath)
        {
            var tsvFile = new TsvFile<T>(filePath);

            if (!ColumnNames.SequenceEqual(tsvFile.ColumnNames))
            {
                throw new ArgumentException("Cannot append files with different column names.");
            }

            _rows.AddRange(tsvFile._rows);
        }

        public void SaveToFile(string filePath)
        {
            var lines = new List<string> { string.Join("\t", ColumnNames) };

            foreach (var row in _rows)
            {
                var rowValues = new List<string>();

                foreach (var columnName in ColumnNames)
                {
                    var property = typeof(T).GetProperty(columnName);
                    var value = property.GetValue(row).ToString();
                    rowValues.Add(value);
                }

                lines.Add(string.Join("\t", rowValues));
            }

            File.WriteAllLines(filePath, lines);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var row in _rows)
            {
                yield return row;
            }
        }
    }
}