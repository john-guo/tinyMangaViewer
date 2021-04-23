using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace tinyMangaViewer
{
    public class EventCommandBehavior : Behavior<UIElement>
    {
        private Delegate _handler;
        private EventInfo _oldEvent;

        public string Event { get { return (string)GetValue(EventProperty); } set { SetValue(EventProperty, value); } }
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(string), typeof(EventCommandBehavior), new PropertyMetadata(null, OnEventChanged));

        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommandBehavior), new PropertyMetadata(null));

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ecb = (EventCommandBehavior)d;
            if (ecb.AssociatedObject != null)
                ecb.AttachHandler((string)e.NewValue);
        }

        protected override void OnAttached()
        {
            AttachHandler(this.Event);
        }

        private void AttachHandler(string eventName)
        {
            if (_oldEvent != null)
                _oldEvent.RemoveEventHandler(AssociatedObject, _handler);

            if (!string.IsNullOrWhiteSpace(eventName))
            {
                EventInfo ei = AssociatedObject.GetType().GetEvent(eventName);
                if (ei != null)
                {
                    MethodInfo mi = GetType().GetMethod("ExecuteCommand", BindingFlags.Instance | BindingFlags.NonPublic);
                    _handler = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                    ei.AddEventHandler(AssociatedObject, _handler);
                    _oldEvent = ei;
                }
            }
        }

        private void ExecuteCommand(object sender, EventArgs e)
        {
            if (Command == null)
                return;
            if (Command.CanExecute(e))
                Command.Execute(e);
        }
    }
}
