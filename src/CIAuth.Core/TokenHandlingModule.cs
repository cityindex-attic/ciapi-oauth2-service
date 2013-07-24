using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Infrastructure
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff649096.aspx
    /// 
    /// </summary>
    public class TokenHandlingModule : IHttpModule
    {
        private HttpApplication _context;
        public void Init(HttpApplication context)
        {
            _context = context;
            _context.PreRequestHandlerExecute += context_PreRequestHandlerExecute;

        }

        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            try
            {
                if (_context != null)
                {
                    _context.Dispose();
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
                // swallow
            }
        }
    }
}
