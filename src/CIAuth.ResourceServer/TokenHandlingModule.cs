using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CIAuth.ResourceServer
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff649096.aspx
    /// 
    /// The purpose of this module is to intercept incoming request in order to translate CIAuth (OAuth2) tokens into 
    /// username and password credentials that the TradingAPI expects.
    /// 
    /// 
    /// 
    /// </summary>
    public class TokenHandlingModule : IHttpModule
    {
        private HttpApplication _context;
        private TokenHandler _handler;
        public void Init(HttpApplication context)
        {
            _handler = new TokenHandler();
            _context = context;
            _context.PreRequestHandlerExecute += context_PreRequestHandlerExecute;

        }

        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            try
            {
                _handler.ProcessRequest(new HttpRequestWrapper(_context.Request));
            }
            catch (Exception)
            {
                // #TODO: set proper exception detail
                throw;
            }
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
