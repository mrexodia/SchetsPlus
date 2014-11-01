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
        private enum ActionType
        {
            Add,
            Remove
        }

        private class UndoAction<U>
        {
            public ActionType Type { get; private set; }
            public U Value { get; private set; }

            public UndoAction(ActionType type, U value)
            {
                this.Type = type;
                this.Value = value;
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
                    list.RemoveAt(list.LastIndexOf(action.Value));
                    break;
                case ActionType.Remove:
                    list.Add(action.Value);
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
                    list.Add(action.Value);
                    break;
                case ActionType.Remove:
                    list.RemoveAt(list.LastIndexOf(action.Value));
                    break;
            }
            return true;
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
            foreach (T value in list)
                addUndoAction(new UndoAction<T>(ActionType.Remove, value));
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
