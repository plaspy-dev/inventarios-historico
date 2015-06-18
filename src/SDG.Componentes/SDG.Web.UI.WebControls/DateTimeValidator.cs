using System;
using System.Web.UI.WebControls;
using SDG.Componentes;

namespace SDG.Web.UI.WebControls;

public class DateTimeValidator : CustomValidator
{
	public DateTime? MaximunValue
	{
		get
		{
			return (DateTime?)Utilidades.GetViewState()[ClientID + "_max"];
		}
		set
		{
			Utilidades.GetViewState()[ClientID + "_max"] = value;
		}
	}

	public DateTime? MinumunValue
	{
		get
		{
			return (DateTime?)Utilidades.GetViewState()[ClientID + "_min"];
		}
		set
		{
			Utilidades.GetViewState()[ClientID + "_min"] = value;
		}
	}

	public DateTimeValidator()
	{
		base.ServerValidate += OnValidate_DateTime;
		base.ClientValidationFunction = "onValidate_DateTime";
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		if (!MaximunValue.HasValue && !MinumunValue.HasValue)
		{
			throw new Exception("No se ha definido ningun valor para MinumunValue o MaximunValue (" + ID + ")");
		}
		if (MinumunValue.HasValue && MaximunValue.HasValue && MaximunValue.Value < MinumunValue.Value)
		{
			throw new Exception("El valor MaximunValue no puede ser menor al MinumunValue (" + ID + ")");
		}
		if (MinumunValue.HasValue)
		{
			Page.ClientScript.RegisterExpandoAttribute(ClientID, "minimunValue", MinumunValue.Value.ToString("dd/MM/yyyy HH:mm"));
		}
		if (MaximunValue.HasValue)
		{
			Page.ClientScript.RegisterExpandoAttribute(ClientID, "maximunValue", MaximunValue.Value.ToString("dd/MM/yyyy HH:mm"));
		}
		if (!Page.ClientScript.IsClientScriptIncludeRegistered("datetimevalidator"))
		{
			Page.ClientScript.RegisterClientScriptInclude("datetimevalidator", Page.ClientScript.GetWebResourceUrl(GetType(), "SDG.Componentes.DateTimeValidator.js"));
		}
	}

	public void OnValidate_DateTime(object sender, ServerValidateEventArgs args)
	{
		if (!MaximunValue.HasValue && !MinumunValue.HasValue)
		{
			throw new Exception("No se ha definido ningun valor para MinumunValue o MaximunValue (" + ID + ")");
		}
		try
		{
			DateTime dateTime = DateTime.Parse(args.Value);
			if (MaximunValue.HasValue && dateTime > MaximunValue.Value)
			{
				args.IsValid = false;
				base.ErrorMessage = "Por favor ingrese una fecha menor a " + MaximunValue;
				return;
			}
			if (MinumunValue.HasValue)
			{
				if (dateTime.Hour == 0 && dateTime.Minute == 0)
				{
					dateTime = dateTime.AddHours(23.0).AddMinutes(59.0).AddSeconds(59.0);
				}
				if (dateTime < MinumunValue.Value)
				{
					args.IsValid = false;
					base.ErrorMessage = "Por favor ingrese una fecha mayor a " + MinumunValue;
					return;
				}
			}
			args.IsValid = true;
		}
		catch
		{
			args.IsValid = false;
		}
	}
}
