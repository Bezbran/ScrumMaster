using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumMasterWcf
{
    /// <summary>
    /// Represent special exceptions in the server with initial error code
    /// </summary>
    [Serializable]
    public class CustomException:Exception
    {
        /// <summary>
        /// Stores the code of the exception
        /// </summary>
        public int ExceptionCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">The message that explain the exception</param>
        /// <param name="exCode">The code of the execpetion</param>
        public CustomException(string msg, int exCode):base(msg)
        {
            this.ExceptionCode = exCode;
        }

        // Codes: 
        // CustomException("There is no Manager.", 111)
        // CustomException("Manager user is already exist.",112)
        // CustomException("Wrong password", 113)
        // CustomException("The user-name already exist", 114)
        // CustomException("Scrum master user is already exist.", 115)
        // CustomException("Product-owner user is already exist.", 116)
        // CustomException("User-name cannot be number", 117)
    }
}
