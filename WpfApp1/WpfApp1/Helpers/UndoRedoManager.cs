using System;
using System.Collections.Generic;
using WpfApp1.Mvvm;

namespace WpfApp1.Helpers
{
    public class UndoRedoManager
    {
        Stack<ICommand> UndoStack { get; set; }
        Stack<ICommand> RedoStack { get; set; }

        public event EventHandler StateChanged;

        public UndoRedoManager()
        {
            UndoStack = new Stack<ICommand>();
            RedoStack = new Stack<ICommand>();
        }

        public void Undo()
        {
            if (UndoStack.Count > 0)
            {
                //изымаем команду из стека
                var command = UndoStack.Pop();
                //отменяем действие команды
                command.UnExecute();
                //заносим команду в стек Redo
                RedoStack.Push(command);
                //сигнализируем об изменениях
                StateChanged?.Invoke(this, null);
            }
        }

        public void Redo()
        {
            if (RedoStack.Count > 0)
            {
                //изымаем команду из стека
                var command = RedoStack.Pop();
                //выполняем действие команды
                command.Execute();
                //заносим команду в стек Undo
                UndoStack.Push(command);
                //сигнализируем об изменениях
                StateChanged?.Invoke(this, null);
            }
        }

        //выполняем команду
        public void Execute(ICommand command)
        {
            //выполняем команду
            command.Execute();
            //заносим в стек Undo
            UndoStack.Push(command);
            //очищаем стек Redo
            RedoStack.Clear();
            //сигнализируем об изменениях
            StateChanged?.Invoke(this, null);
        }
    }
}
