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
            public bool Add { get; private set; }
            public U Value { get; private set; }

            public UndoAction(bool add, U value)
            {
                this.Add = add;
                this.Value = value;
            }
        }

        public List<T> list { get; private set; }
        private List<UndoAction<T>> undoActions = new List<UndoAction<T>>();
        private int pointer = -1;

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

        private void addUndoAction(UndoAction<T> action)
        {
            if (pointer != -1)
                undoActions.RemoveRange(pointer + 1, undoActions.Count - pointer - 1);
            undoActions.Add(action);
            pointer = undoActions.Count - 1;
        }

        public bool Undo()
        {
            if (pointer == -1)
                return false;
            UndoAction<T> action = undoActions[pointer];
            pointer--;
            if (action.Add)
                list.RemoveAt(list.LastIndexOf(action.Value));
            else
                list.Add(action.Value);
            return true;
        }

        public bool Redo()
        {
            if (pointer == undoActions.Count - 1)
                return false;
            pointer++;
            UndoAction<T> action = undoActions[pointer];
            if (action.Add)
                list.Add(action.Value);
            else
                list.Remove(action.Value);
            return true;
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void Add(T value)
        {
            list.Add(value);
            addUndoAction(new UndoAction<T>(true, value));
        }

        public void RemoveAt(int index)
        {
            addUndoAction(new UndoAction<T>(false, list[index]));
            list.RemoveAt(index);
        }

        public void Clear()
        {
            foreach (T value in list)
            {
                undoActions.Add(new UndoAction<T>(false, value));
                pointer++;
            }
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
