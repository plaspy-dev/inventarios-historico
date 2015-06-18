using System;

namespace SDG.Componentes;

public class PrintEventArgs : EventArgs
{
	public string Contenido = "";

	public bool pdf = false;

	public PrintEventArgs()
	{
	}

	public PrintEventArgs(bool pdf)
	{
		this.pdf = pdf;
	}

	public void ReplaceVariable(string variable, string valor)
	{
		Contenido = Contenido.Replace(variable, valor);
	}
}
