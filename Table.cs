using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics.Tables {
    public class Table<TRow, TCol, TValue> { // класс таблицы с произвольными типами данных

        public List<TRow> Rows = new List<TRow>(); // строки таблицы
        public List<TCol> Columns = new List<TCol>(); // столбцы таблицы
        private Dictionary<TRow, Dictionary<TCol, TValue>> tableCellValues; // словарь, хранящий значения ячеек таблицы

        public TValue DefaultValue { get; set; } // свойство, задаваёт значение по умолчанию
        
        public Table<TRow, TCol, TValue> Open { // свойство, ссылка на сам класс Table
            get {
                return this;
            }
        }
        
        public void AddRow(TRow row) { // метод добавления строк в таблицу
            if (!this.Rows.Contains(row)) {
                this.Rows.Add(row);
            }
        }

        public void AddColumn(TCol column) { // метод добавления столбцов в таблицу
            if (!this.Columns.Contains(column)) {
                this.Columns.Add(column);
            }
        }

        public TValue this[TRow row, TCol col] { // индексатор для доступа к ячейке таблицы по ее координатам
            get {

                if (!this.tableCellValues.ContainsKey(row) || !this.tableCellValues[row].ContainsKey(col)) { // ячейка не имеет значения?
                    return this.DefaultValue; // не имеет - возвращается DefaultValue
                } 
                return this.tableCellValues[row][col]; // имеет - оно возвращается
            }
            set {
                if (value.GetType() != typeof(TValue)) { // тип значения не TValue
                    throw new ArgumentException(); // исключение
                }
                this.AddRow(row); // ячейки не существует, добавляем строки в таблицу
                this.AddColumn(col); // ячейки не существует, добавляем столбцы в таблицу

                if (!this.tableCellValues.ContainsKey(row)) { // для строки нет словаря
                    this.tableCellValues[row] = new Dictionary<TCol, TValue>(); // создаём словарь
                }

                this.tableCellValues[row][col] = value; // записываем значение
            }
        }

        public bool RowExists(TRow row) { // метод проверки наличия строк в таблице
            return Rows.Contains(row) && Rows.Count > 0;

        }

        public bool ColExists(TCol col) { // метод проверки наличия столбцов в таблице
            return Columns.Contains(col) && Columns.Count > 0;
        }

        public bool Exists(TRow row, TCol col) { // метод проверки ячеек столбцов в таблице
            return RowExists(row) && ColExists(col);
        }

        public TableAccessor<TRow, TCol, TValue> Existed { // свойство, возвращает объект для работы с существующими в таблице значениями
            get {
                return new TableAccessor<TRow, TCol, TValue>(this); 
            }
        }

        // конструктор класса Table
        public Table() { // конструктор, создает пустой словарь, задает значение по умолчанию для ячеек таблицы
            this.tableCellValues = new Dictionary<TRow, Dictionary<TCol, TValue>>(); // создает пустой словарь
            this.DefaultValue = default(TValue); // задает значение по умолчанию для ячеек таблицы
        }
    }

    public class TableAccessor<TRow, TCol, TValue> { // класс чтения и записи значений в существующие в таблице ячейки
        
        private Table<TRow, TCol, TValue> tableObject; // поле для хранения ссылки на объект класса

        public TValue this[TRow row, TCol col] { // индексатор доступа к ячейке таблицы по ее координатам
            get {
                if (!tableObject.Exists(row, col)) { // ячейки с заданными координатами не существует
                    throw new ArgumentException(); // исключение
                }

                return tableObject[row, col]; // ячейка существует - ее значение возвращается
            }
            set {
                if (!tableObject.Exists(row, col)) { // ячейки с заданными координатами не существует
                    throw new ArgumentException(); // исключение
                }

                tableObject[row, col] = value; // проверки пройдены, значение записывается
            }
        }

        public TableAccessor(Table<TRow, TCol, TValue> table) { // конструктор класса TableAccessor
            this.tableObject = table;
        }
    }
}
