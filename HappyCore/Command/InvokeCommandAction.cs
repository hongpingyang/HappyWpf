using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace HappyCore
{
    /// <summary>
    /// 命令->ViewModel
    /// </summary>
    public class InvokeCommandAction : TriggerAction<UIElement>
    {
        /// <summary>
        /// 命令
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand),
                typeof(InvokeCommandAction), null);

        /// <summary>
        /// 命令参数
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter",
                typeof(object), typeof(InvokeCommandAction), null);

        /// <summary>
        /// 命令
        /// </summary>
        public ICommand Command
        {
             get
             {
                return  (ICommand)GetValue(CommandProperty);
             }
             set
             {
                   SetValue(CommandProperty, value);
             }
        }
        /// <summary>
        /// 命令参数
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject != null)
            {
                if (Command != null)
                {
                    //这里添加事件触发源和事件参数
                    CommandParameter exParameter = new CommandParameter
                    {
                        Sender = AssociatedObject,
                        Parameter = GetValue(CommandParameterProperty),
                        EventArgs = parameter as EventArgs
                    };

                    if (Command != null && Command.CanExecute(exParameter))
                    {
                        //将扩展的参数传递到Execute方法中
                        Command.Execute(exParameter);
                    }
                }
            }
        }
    }
}
