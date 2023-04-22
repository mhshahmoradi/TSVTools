using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TSVTools
{
    public class TsvFile<T> : IEnumerable<T> where T : new()
    {
        private readonly List<T> _rows = new List<T>();

        private List<string> ColumnNames { get; set; }

        public int Count => _rows.Count;

        public TsvFile()
        {
            ColumnNames = typeof(T).GetProperties().Select(p => p.Name).ToList();
        }

        public TsvFile(string filePath) : this()
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            LoadFromFile(filePath);
        }

        private void LoadFromFile(string filePath)
        {
            using var reader = new StreamReader(filePath);

            if (reader.EndOfStream)
            {
                throw new ArgumentException($"File '{filePath}' is empty.");
            }

            ColumnNames = new List<string>(reader.ReadLine().Split('\t'));
            int i = 1;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var rowValues = line.Split('\t');

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
                    object typedValue;

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

        public void ClearRows()
        {
            _rows.Clear();
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
            using var writer = new StreamWriter(filePath);

            writer.WriteLine(string.Join("\t", ColumnNames));

            foreach (var row in _rows)
            {
                var rowValues = new List<string>();

                foreach (var columnName in ColumnNames)
                {
                    var property = typeof(T).GetProperty(columnName);
                    var value = property.GetValue(row).ToString();
                    rowValues.Add(value);
                }

                writer.WriteLine(string.Join("\t", rowValues));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}