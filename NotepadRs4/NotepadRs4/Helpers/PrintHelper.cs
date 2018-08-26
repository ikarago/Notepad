using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace NotepadRs4.Helpers
{
    public class PrintHelper
    {
        protected PrintDocument printDocument;
        protected IPrintDocumentSource printDocumentSource;

        protected FrameworkElement firstPage;




        public virtual void RegisterForPrinting()
        {
            printDocument = new PrintDocument();
            printDocumentSource = printDocument.DocumentSource;
            // Paginate
            printDocument.Paginate += Paginate;
            // Get Preview Pages
            // Add pages

            PrintManager printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested += PrintTaskRequested;
        }

        private void Paginate(object sender, PaginateEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual void UnregisterForPrinting()
        {
            if (printDocument == null)
            {
                return;
            }

            // Paginate
            // Get preview page
            // add pages

            PrintManager printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested -= PrintTaskRequested;

            // clear canvas
        }

        public virtual void PreparePrintContent(Page page)
        {
            if (firstPage == null)
            {
                firstPage = page;
            }

            // Add it to the print canvas
        }




        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("Notepad", sourceRequested =>
            {
                printTask.Completed += async (s, args) =>
                {
                    if (args.Completion == PrintTaskCompletion.Failed)
                    {
                        Debug.WriteLine("Printing failed");
                        // Notify back to the user

                    }
                };

                sourceRequested.SetSource(printDocumentSource);
            });
        }
    }
}
