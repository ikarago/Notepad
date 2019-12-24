using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Build with the sample of David Ewen at StackOverflow: http://stackoverflow.com/questions/33290361/

namespace NotepadRs4.Controls
{
    // #ACTIVE CODE (AGAIN): This code isn't used anymore in this app, but it's still here so you might be able to learn something from it
    //      Reason for abandoning this code is due to the CommandBar being able to function properly again, so no need for it anymore to force the DropDownMenu to go down.
    //          UPDATE: 2019-11-04: After some UI updates, the CommandBar goes ugly again, so I re-enabled this to be able to make use of it again.
    
    public class OpenDownCommandBarVisualStateManager : VisualStateManager
    {
        protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            // Replace the OpenUp state for the OpenDown one and continue like normal
            if (!string.IsNullOrWhiteSpace(stateName) && stateName.EndsWith("OpenUp"))
            {
                stateName = stateName.Substring(0, stateName.Length - 6) + "OpenDown";
            }

            return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
        }
    }

    public class OpenDownCommandBar : CommandBar
    {
        public OpenDownCommandBar()
        {

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var layoutRoot = GetTemplateChild("LayoutRoot") as Grid;
            if (layoutRoot != null)
            {
                VisualStateManager.SetCustomVisualStateManager(layoutRoot, new OpenDownCommandBarVisualStateManager());
            }
        }
    } 
}
