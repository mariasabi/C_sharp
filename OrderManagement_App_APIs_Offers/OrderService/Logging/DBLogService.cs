
using log4net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OrderService.DTOs;
using OrderService.Models;
using System.Data;



namespace OrderService.Logging
{
  
        public class DBLogService
        {
            private readonly OrderContext _context;
            private static readonly ILog log = LogManager.GetLogger(typeof(DBLogService));
            private readonly IHttpContextAccessor _httpContextAccessor;
            private string _userId;
            //private readonly HttpContext _httpContext;

            public DBLogService(OrderContext context, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;
            //_httpContextAccessor = httpContextAccessor;
            // _userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            //ThreadContext.Properties["UserId"] = Int32.Parse(_userId);

            var user = _httpContextAccessor.HttpContext.Items["User"];
            _userId = user.ToString();
            //_userId =Int.Parse(user);

            }
            public void CallStoredProcedure(DateTime date, string thread, string level, string logger, string message)
            {
                //var parameter = new SqlParameter("@ParameterName", 1);
                try
                {
                    var dateParameter = new SqlParameter("@Date", SqlDbType.DateTime) { Value = date };
                    var threadParameter = new SqlParameter("@Thread", SqlDbType.NVarChar, 255) { Value = thread };
                    var levelParameter = new SqlParameter("@Level", SqlDbType.NVarChar, 50) { Value = level };
                    var loggerParameter = new SqlParameter("@Logger", SqlDbType.NVarChar, 255) { Value = logger };
                    var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, -1) { Value = message };
                    var userIdParameter = new SqlParameter("@UserId", SqlDbType.Int) { Value = _userId };
                    _context.Database.ExecuteSqlRaw("EXEC LogError @Date, @Thread, @Level, @Logger, @Message, @UserId", dateParameter, threadParameter, levelParameter, loggerParameter, messageParameter, userIdParameter);
                }
                catch
                {
                    log.Debug("SQL exception while adding logs to database has occurred for User Id " + _userId);
                }
            }
        }
    

}
