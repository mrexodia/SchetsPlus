using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchetsEditor
{
    public class UndoList<T> : IEnumerable
    {
        private class UndoAction<U>
        {
            private bool add = false;
            private U value;

            public UndoAction(bool add, U value)
            {
                this.add = add;
                this.value = value;
            }

            public static implicit operator U(UndoAction<U> undoAction)
            {
                return undoAction.value;
            }
        }

        private List<T> list;
        private List<UndoAction<T>> undoActions = new List<UndoAction<T>>();

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public UndoList(List<T> list)
        {
            this.list = list;
        }

        public static implicit operator List<T>(UndoList<T> undoList)
        {
            return undoList.list;
        }

        public void Undo()
        {
        }

        public void Redo()
        {
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void Add(T value)
        {
            list.Add(value);
            undoActions.Add(new UndoAction<T>(true, value));
        }

        public void RemoveAt(int index)
        {
            undoActions.Add(new UndoAction<T>(false, list[index]));
            list.RemoveAt(index);
        }

        public void Clear()
        {
            foreach (T value in list)
                undoActions.Add(new UndoAction<T>(false, value));
            list.Clear();
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }
    }
}
