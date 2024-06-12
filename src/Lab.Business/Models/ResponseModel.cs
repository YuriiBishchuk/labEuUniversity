using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab.Business.Models
{
    internal class ResponseModel<T>
    {
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
