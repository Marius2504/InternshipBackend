using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class ResponseDTO
    {
        public ResponseDTO(string message)
        {
            this.Message = message;
        }
        public string Message { get; set; }
    }
}
