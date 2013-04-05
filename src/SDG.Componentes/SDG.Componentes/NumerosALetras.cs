using System;

namespace SDG.Componentes;

public class NumerosALetras
{
	private static string[] m_saCeroAVeinte = new string[20]
	{
		"cero", "un", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve",
		"diez", "once", "doce", "trece", "catorce", "quince", "dieciseis", "diecisiete", "dieciocho", "diecinueve"
	};

	private static string[] m_saCientos = new string[10] { "", "ciento", "docientos", "trecientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos" };

	private static string[] m_saDeca = new string[8] { "veinte", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };

	private static string[] m_saMasDeMil = new string[5] { "", "mil", "millon", "mil millones", "billon" };

	protected static string convert_nn(int val)
	{
		if (val < 20)
		{
			return m_saCeroAVeinte[val];
		}
		for (int i = 0; i < 10; i++)
		{
			string text = m_saDeca[i];
			int num = 20 + 10 * i;
			if (num + 10 > val)
			{
				if (val % 10 != 0)
				{
					return text + " y " + m_saCeroAVeinte[val % 10];
				}
				return text;
			}
		}
		return "";
	}

	protected static string convert_nnn(int val)
	{
		string text = "";
		int num = val / 100;
		int num2 = val % 100;
		if (num > 0)
		{
			text = m_saCientos[num];
			if (num2 > 0)
			{
				text += " ";
			}
		}
		if (num2 > 0)
		{
			text += convert_nn(num2);
		}
		else if (num < 2)
		{
			text.Remove(text.Length - 1);
			text.Remove(text.Length - 1);
		}
		return text;
	}

	public static string ToLetras(double valor)
	{
		string text = ToLetras(valor, comas: false);
		if (text.Length > 0)
		{
			text = text.Substring(0, 1).ToUpper() + text.Substring(1);
		}
		return text;
	}

	public static string ToLetras(double valor, bool comas)
	{
		valor = Math.Floor(valor);
		if (valor < 100.0)
		{
			return convert_nn(Convert.ToInt32(valor));
		}
		if (valor < 1000.0)
		{
			return convert_nnn(Convert.ToInt32(valor));
		}
		for (int i = 1; i < 5; i++)
		{
			double num = Math.Pow(1000.0, i);
			if (num > valor)
			{
				int num2 = i - 1;
				double num3 = Math.Pow(1000.0, num2);
				double num4 = Math.Floor(valor / num3);
				double num5 = valor - num4 * num3;
				string text = ((num2 > 1 && num4 != 1.0) ? (convert_nnn(Convert.ToInt32(num4)) + " " + m_saMasDeMil[num2] + "es") : ((num4 != 1.0) ? (convert_nnn(Convert.ToInt32(num4)) + " " + m_saMasDeMil[num2]) : ((num2 != 1 && num2 != 3) ? (convert_nnn(Convert.ToInt32(num4)) + " " + m_saMasDeMil[num2]) : m_saMasDeMil[num2])));
				if (num5 > 0.0)
				{
					text = ((!comas) ? (text + " " + ToLetras(num5)) : (text + ", " + ToLetras(num5)));
				}
				return text;
			}
		}
		return "";
	}
}
