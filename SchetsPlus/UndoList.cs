using System.Collections;
using System.Collections.Generic;

namespace SchetsPlus
{
    public class UndoList<T> : IEnumerable
    {
        private enum ActionType
        {
            Add,
            Remove,
            Swap,
            Replace
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
                case ActionType.Swap:
                    {
                        for (int i = action.Values.Count - 1; i >= 0; i -= 2)
                        {
                            T value1 = action.Values[i];
                            T value2 = action.Values[i - 1];
                            list[list.IndexOf(value2)] = value1;
                            list[list.IndexOf(value1)] = value2;
                        }
                    }
                    break;
                case ActionType.Replace:
                    list[list.IndexOf(action.Values[1])] = action.Values[0];
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
                case ActionType.Swap:
                    {
                        for (int i = action.Values.Count - 1; i >= 0; i -= 2)
                        {
                            T value1 = action.Values[i - 1];
                            T value2 = action.Values[i];
                            list[list.IndexOf(value2)] = value1;
                            list[list.IndexOf(value1)] = value2;
                        }
                    }
                    break;
                case ActionType.Replace:
                    list[list.IndexOf(action.Values[0])] = action.Values[1];
                    break;
            }
            return true;
        }

        public int GetListHash()
        {
            int hash = 0;
            for (int i = 0; i < list.Count; i++)
                hash += list[i].GetHashCode() * (i + 1);
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

        public void Swap(int index1, int index2)
        {
            T value1 = list[index1];
            T value2 = list[index2];
            list[index1] = value2;
            list[index2] = value1;
            addUndoAction(new UndoAction<T>(ActionType.Swap, new List<T>() { value1, value2 }));
        }

        public void MoveToFront(int index)
        {
            List<T> actionList = new List<T>();
            for (int i = index; i >= 1; i--)
            {
                T value1 = list[i];
                T value2 = list[i - 1];
                list[i] = value2;
                list[i - 1] = value1;
                actionList.Add(value1);
                actionList.Add(value2);
            }
            addUndoAction(new UndoAction<T>(ActionType.Swap, actionList));
        }

        public void MoveToBack(int index)
        {
            List<T> actionList = new List<T>();
            for (int i = index; i < list.Count - 1; i++)
            {
                T value1 = list[i];
                T value2 = list[i + 1];
                list[i] = value2;
                list[i + 1] = value1;
                actionList.Add(value1);
                actionList.Add(value2);
            }
            addUndoAction(new UndoAction<T>(ActionType.Swap, actionList));
        }

        public void Replace(int index, T value)
        {
            T oldvalue = list[index];
            list[index] = value;
            addUndoAction(new UndoAction<T>(ActionType.Replace, new List<T>() { oldvalue, value }));
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                Replace(index, value);
            }
        }
    }
}
