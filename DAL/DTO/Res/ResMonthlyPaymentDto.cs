﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResMonthlyPaymentDto
    {
        public string Id { get; set; }
        public string RepaymentId { get; set; }
        public decimal Amount { get; set; }
        public bool status { get; set; }
    }
}
