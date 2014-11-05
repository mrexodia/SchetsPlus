using System.Collections;
using System.Collections.Generic;

namespace SchetsPlus
{
    public class UndoList<T> : IEnumerable
    {
        private enum ActionType
        {
            Add,
            Remove
        }

        private class UndoAction<U>
        {
            public ActionType Type { get; private set; }
            public List<U> Values { get; private set; }

            public UndoAction(ActionType type, U value)
            {
                this.Type = type;
                this.Values = new List<U>();
                this.Values.Add(value);
            }

            public UndoAction(ActionType type, List<U> values)
            {
                this.Type = type;
                this.Values = new List<U>(values);
            }
        }

        private List<T> list;
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
            switch (action.Type)
            {
                case ActionType.Add:
                    foreach (T value in action.Values)
                        list.RemoveAt(list.LastIndexOf(value));
                    break;
                case ActionType.Remove:
                    foreach (T value in action.Values)
                        list.Add(value);
                    break;
            }
            return true;
        }

        public bool Redo()
        {
            if (pointer == undoActions.Count - 1)
                return false;
            pointer++;
            UndoAction<T> action = undoActions[pointer];
            switch (action.Type)
            {
                case ActionType.Add:
                    foreach (T value in action.Values)
                        list.Add(value);
                    break;
                case ActionType.Remove:
                    foreach (T value in action.Values)
                        list.RemoveAt(list.LastIndexOf(value));
                    break;
            }
            return true;
        }

        public int GetListHash()
        {
            int hash = 0;
            foreach (T value in list)
                hash += value.GetHashCode();
            return hash;
        }

        public IEnumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void Add(T value)
        {
            list.Add(value);
            addUndoAction(new UndoAction<T>(ActionType.Add, value));
        }

        public void RemoveAt(int index)
        {
            addUndoAction(new UndoAction<T>(ActionType.Remove, list[index]));
            list.RemoveAt(index);
        }

        public void Clear()
        {
            addUndoAction(new UndoAction<T>(ActionType.Remove, list));
            list.Clear();
        }

        public List<T> CopyList()
        {
            return new List<T>(list);
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
