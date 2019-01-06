﻿using Mediachase.Commerce.Orders.Dto;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AcmePaymentProvider
{
    public partial class ConfigurePayment : OrderBaseUserControl, IGatewayControl
    {
        private PaymentMethodDto paymentMethodDto = null;
        private const string secretKeyParamName = "SecretKeyExample";
        public string ValidationGroup { get; set; } = string.Empty;

        public void LoadObject(object dto)
        {
            paymentMethodDto = dto as PaymentMethodDto;
        }

        public void SaveChanges(object dto)
        {
            if (Visible)
            {
                paymentMethodDto = dto as PaymentMethodDto;
                if(paymentMethodDto != null && paymentMethodDto.PaymentMethodParameter != null)
                {
                    var paymentMethodId = Guid.Empty;
                    if(paymentMethodDto.PaymentMethod.Count > 0)
                    {
                        paymentMethodId = paymentMethodDto.PaymentMethod[0].PaymentMethodId;
                    }
                    var param = GetParameterByName(secretKeyParamName);
                    if(param != null)
                    {
                        param.Value = txtSecretKey.Text;
                    }
                    else
                    {
                        var newRow = paymentMethodDto.PaymentMethodParameter.NewPaymentMethodParameterRow();
                        newRow.PaymentMethodId = paymentMethodId;
                        newRow.Parameter = secretKeyParamName;
                        newRow.Value = txtSecretKey.Text;
                        paymentMethodDto.PaymentMethodParameter.Rows.Add(newRow);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //DataBind();
        }

        public override void DataBind()
        {
            if (paymentMethodDto != null && paymentMethodDto.PaymentMethodParameter != null)
            {
                var param = GetParameterByName(secretKeyParamName);
                if (param != null)
                {
                    txtSecretKey.Text = param.Value;
                }
                else
                {
                    Visible = false;
                }
            }
            base.DataBind();
        }

        private PaymentMethodDto.PaymentMethodParameterRow GetParameterByName(string name)
        {
            var rows = paymentMethodDto.PaymentMethodParameter.Select($"Parameter='{name}'").First();
            return rows as PaymentMethodDto.PaymentMethodParameterRow;
        }
    }
}