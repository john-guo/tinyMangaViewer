using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace tinyMangaViewer
{
    public class MouseWheelGesture : MouseGesture
    {
        public static MouseWheelGesture Down => new MouseWheelGesture { Direction = WheelDirection.Down };
        public static MouseWheelGesture Up => new MouseWheelGesture { Direction = WheelDirection.Up };
        public static MouseWheelGesture CtrlUp => new MouseWheelGesture(ModifierKeys.Control) { Direction = WheelDirection.Up };
        public static MouseWheelGesture CtrlDown => new MouseWheelGesture(ModifierKeys.Control) { Direction = WheelDirection.Down };

        public MouseWheelGesture() : base(MouseAction.WheelClick)
        {
        }

        public MouseWheelGesture(ModifierKeys modifiers) : base(MouseAction.WheelClick, modifiers)
        {
        }

        public WheelDirection Direction { get; set; }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            if (!(inputEventArgs is MouseWheelEventArgs args)) return false;
            switch (Direction)
            {
                case WheelDirection.None:
                    return args.Delta == 0;
                case WheelDirection.Up:
                    return args.Delta > 0;
                case WheelDirection.Down:
                    return args.Delta < 0;
                default:
                    return false;
            }
        }

        public enum WheelDirection
        {
            None,
            Up,
            Down,
        }
    }
}
