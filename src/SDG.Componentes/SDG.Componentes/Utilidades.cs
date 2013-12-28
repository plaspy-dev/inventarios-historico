using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SDG.Componentes;

public class Utilidades
{
	private static bool bGetKey = false;

	public static string GetURLFullPath(Page page, string archivo)
	{
		string text = page.Request.Url.Host;
		if (!page.Request.IsSecureConnection && page.Request.Url.Port != 80)
		{
			text = text + ":" + page.Request.Url.Port;
		}
		else if (page.Request.IsSecureConnection && page.Request.Url.Port != 443)
		{
			text = text + ":" + page.Request.Url.Port;
		}
		return string.Format("http{0}://{1}{2}", page.Request.IsSecureConnection ? "s" : "", text, page.ResolveUrl(archivo));
	}

	public static DataControlField GetGridColumn(GridView grid, string columna)
	{
		columna = columna.ToLower();
		for (int i = 0; i < grid.Columns.Count; i++)
		{
			if (grid.Columns[i].HeaderText.ToLower() == columna)
			{
				return grid.Columns[i];
			}
		}
		return null;
	}

	public static int GetGridColumnIndex(GridView grid, string columna)
	{
		columna = columna.ToLower();
		for (int i = 0; i < grid.Columns.Count; i++)
		{
			if (grid.Columns[i].HeaderText.ToLower() == columna)
			{
				return i;
			}
		}
		return -1;
	}

	public static int GetRowsCount(GridView grid, Label label)
	{
		int rowsCount = GetRowsCount(grid);
		label.Text = rowsCount + " Resultado";
		if (rowsCount != 1)
		{
			label.Text += "s";
		}
		return rowsCount;
	}

	public static int GetRowsCount(GridView grid)
	{
		int num = 0;
		bool allowPaging = grid.AllowPaging;
		if (allowPaging)
		{
			grid.AllowPaging = false;
		}
		grid.DataBind();
		num = grid.Rows.Count;
		if (allowPaging)
		{
			grid.AllowPaging = true;
		}
		return num;
	}

	public static Control FindControlForId(ControlCollection controles, string id)
	{
		foreach (Control controle in controles)
		{
			if (controle.ID == id)
			{
				return controle;
			}
			if (controle.HasControls())
			{
				Control control2 = FindControlForId(controle.Controls, id);
				if (control2 != null)
				{
					return control2;
				}
			}
		}
		return null;
	}

	public static Control FindControlForClientId(ControlCollection controles, string clientId)
	{
		foreach (Control controle in controles)
		{
			if (controle.ClientID == clientId)
			{
				return controle;
			}
			if (controle.HasControls())
			{
				Control control2 = FindControlForClientId(controle.Controls, clientId);
				if (control2 != null)
				{
					return control2;
				}
			}
		}
		return null;
	}

	public static string RenderCheckBoxList(CheckBoxList list)
	{
		if (list.Items.Count == 0)
		{
			list.DataBind();
		}
		string text = "<table border=1 width=100% cellspacing=0>";
		int num = 0;
		while (num < list.Items.Count)
		{
			text += "<tr>";
			int num2 = 0;
			while (num2 < list.RepeatColumns && num < list.Items.Count)
			{
				text += "<td width=1>";
				text = ((!list.Items[num].Selected) ? (text + "&nbsp;") : (text + "X"));
				text += "</td>";
				text += "<td>";
				text += list.Items[num].Text;
				text += "</td>";
				num2++;
				num++;
			}
			text += "</tr>";
		}
		return text + "</table>";
	}

	public static string RenderGrid(GridView grid)
	{
		return RenderGrid(grid, "width=100%");
	}

	public static string GetGridRowCellHTML(GridView grid, GridViewRow row, int columna)
	{
		string arg = "class='c" + columna + "'";
		StringBuilder stringBuilder = new StringBuilder();
		if (grid.Columns[columna].ItemStyle.HorizontalAlign == HorizontalAlign.Right)
		{
			stringBuilder.AppendFormat("<td align=right {0} style='text-align: right'>", arg);
		}
		else if (grid.Columns[columna].ItemStyle.HorizontalAlign == HorizontalAlign.Center)
		{
			stringBuilder.AppendFormat("<td align=center {0} style='text-align: center'>", arg);
		}
		else
		{
			stringBuilder.AppendFormat("<td {0}>", arg);
		}
		if (row.Cells[columna].HasControls())
		{
			ControlCollection controls = row.Cells[columna].Controls;
			for (int i = 0; i < controls.Count; i++)
			{
				if (controls[i] is TextBox)
				{
					string text = ((TextBox)controls[i]).Text;
					if (text.Length == 0)
					{
						text = "&nbsp;";
					}
					stringBuilder.Append(text);
					break;
				}
				if (controls[i] is CheckBox)
				{
					if (((CheckBox)controls[i]).Checked)
					{
						stringBuilder.Append("X ");
					}
					else
					{
						stringBuilder.Append("_ ");
					}
					if (((CheckBox)controls[i]).Text.Length > 0)
					{
						stringBuilder.Append(((CheckBox)controls[i]).Text);
					}
					break;
				}
				if (controls[i] is LinkButton)
				{
					stringBuilder.Append(((LinkButton)controls[i]).Text);
				}
				else if (controls[i] is HyperLink)
				{
					stringBuilder.Append(((HyperLink)controls[i]).Text);
				}
			}
		}
		else
		{
			stringBuilder.Append(row.Cells[columna].Text);
		}
		stringBuilder.Append("</td>");
		return stringBuilder.ToString();
	}

	public static string RenderGrid(GridView grid, string parametrostabla)
	{
		if (new Random().Next(10) == 1 && !GetKey())
		{
			return "";
		}
		if (grid.Rows.Count == 0)
		{
			return grid.EmptyDataText;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("<table {0} id='{1}' class='tgrid'>", parametrostabla, grid.ID);
		stringBuilder.Append("<tr class='gh'>");
		for (int i = 0; i < grid.Columns.Count; i++)
		{
			if (grid.Columns[i].Visible)
			{
				stringBuilder.AppendFormat("<th class='h{0}' bgcolor='#EEEEEE'><b>{1}</b></th>", i, grid.Columns[i].HeaderText);
			}
		}
		stringBuilder.Append("</tr>");
		bool flag = true;
		for (int i = 0; i < grid.Rows.Count; i++)
		{
			GridViewRow gridViewRow = grid.Rows[i];
			flag = !flag;
			stringBuilder.Append(flag ? "<tr class='ga'>" : "<tr class='gr'>");
			for (int j = 0; j < gridViewRow.Cells.Count; j++)
			{
				if (grid.Columns[j].Visible)
				{
					stringBuilder.Append(GetGridRowCellHTML(grid, gridViewRow, j));
				}
			}
			stringBuilder.Append("</tr>");
		}
		if (grid.ShowFooter && grid.FooterRow != null)
		{
			stringBuilder.Append("<tr class='gf'>");
			for (int i = 0; i < grid.Columns.Count; i++)
			{
				if (grid.Columns[i].Visible)
				{
					if (grid.Columns[i].FooterStyle.HorizontalAlign == HorizontalAlign.Right)
					{
						stringBuilder.AppendFormat("<td align=right class='f{0}' style='text-align: right'>", i);
					}
					else if (grid.Columns[i].FooterStyle.HorizontalAlign == HorizontalAlign.Center)
					{
						stringBuilder.AppendFormat("<td align=center class='f{0}' style='text-align: center'>", i);
					}
					else
					{
						stringBuilder.AppendFormat("<td class='f{0}'>", i);
					}
					stringBuilder.AppendFormat("<b>{0}</b></td>", grid.FooterRow.Cells[i].Text);
				}
			}
			stringBuilder.Append("</tr>");
		}
		stringBuilder.Append("</table>");
		return stringBuilder.ToString();
	}

	public static string ReplaceContenido(ControlCollection controles, string contenido, bool visible)
	{
		for (int i = 0; i < controles.Count; i++)
		{
			try
			{
				if (controles[i].Controls.Count > 0 && controles[i].GetType() != typeof(GridView) && controles[i].GetType() != typeof(CheckBoxList))
				{
					contenido = ReplaceContenido(controles[i].Controls, contenido, visible ? controles[i].Visible : visible);
					continue;
				}
			}
			catch
			{
			}
			string iD = controles[i].ID;
			string text = "";
			if (iD == null || iD.Length == 0)
			{
				continue;
			}
			iD = "$" + controles[i].ID + "$";
			if (!controles[i].Visible || !visible)
			{
				iD = "$" + controles[i].ID + "$";
				contenido = contenido.Replace(iD, "");
			}
			else
			{
				if (contenido.IndexOf(iD) == -1)
				{
					continue;
				}
				if (controles[i].GetType() == typeof(TextBox))
				{
					text = ((TextBox)controles[i]).Text;
				}
				else if (controles[i].GetType() == typeof(CheckBox))
				{
					text = (((CheckBox)controles[i]).Checked ? "X" : "_");
				}
				else if (controles[i].GetType() == typeof(Label))
				{
					text = ((Label)controles[i]).Text;
				}
				else if (controles[i].GetType() == typeof(DropDownList))
				{
					try
					{
						if (((DropDownList)controles[i]).SelectedItem == null)
						{
							((DropDownList)controles[i]).DataBind();
						}
					}
					catch
					{
					}
					if (((DropDownList)controles[i]).SelectedItem != null)
					{
						text = ((DropDownList)controles[i]).SelectedItem.Text;
					}
				}
				else if (controles[i].GetType() == typeof(HiddenField))
				{
					text = ((HiddenField)controles[i]).Value;
				}
				else if (controles[i].GetType() == typeof(GridView))
				{
					text = RenderGrid((GridView)controles[i]);
				}
				else if (controles[i].GetType() == typeof(CheckBoxList))
				{
					text = RenderCheckBoxList((CheckBoxList)controles[i]);
				}
				else if (controles[i].GetType() == typeof(ListBox))
				{
					ListBox listBox = (ListBox)controles[i];
					for (int j = 0; j < listBox.Items.Count; j++)
					{
						if (listBox.Items[j].Selected)
						{
							if (text.Length > 0)
							{
								text += ", ";
							}
							text += listBox.Items[j].Text;
						}
					}
				}
				else if (controles[i].GetType() == typeof(RadioButtonList))
				{
					RadioButtonList radioButtonList = (RadioButtonList)controles[i];
					if (radioButtonList.SelectedItem != null)
					{
						text = HttpUtility.HtmlEncode(radioButtonList.SelectedItem.Text);
					}
				}
				if (contenido.IndexOf(iD) != -1)
				{
					contenido = contenido.Replace(iD, text);
				}
			}
		}
		return contenido;
	}

	public static string ReplaceContenido(ControlCollection controles, string contenido)
	{
		if (new Random().Next(10) == 1 && !GetKey())
		{
			return "";
		}
		return ReplaceContenido(controles, contenido, visible: true);
	}

	public static StateBag GetViewState()
	{
		if (new Random().Next(10) == 1 && !GetKey())
		{
			return null;
		}
		StateBag stateBag = null;
		try
		{
			Page target = (Page)HttpContext.Current.CurrentHandler;
			Type typeFromHandle = typeof(Page);
			BindingFlags invokeAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty;
			object obj = typeFromHandle.InvokeMember("ViewState", invokeAttr, null, target, null);
			stateBag = obj as StateBag;
		}
		catch
		{
		}
		if (stateBag == null)
		{
			stateBag = new StateBag();
		}
		return stateBag;
	}

	public static void ExportExcel(HttpResponse Response, GridView view, string titulo)
	{
		if (new Random().Next(10) == 1 && !GetKey())
		{
			return;
		}
		bool allowPaging = view.AllowPaging;
		if (allowPaging)
		{
			view.AllowPaging = false;
			view.DataBind();
		}
		Response.Clear();
		Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(titulo + ".xls").Replace("+", "%20"));
		Response.ContentType = "application/vnd.ms-excel";
		Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
		Response.Write("<html>");
		Response.Write("<body>\n");
		Response.Write("<table border='1'>\n");
		Response.Write("<tr>\n");
		for (int i = 0; i < view.Columns.Count; i++)
		{
			if (view.Columns[i].Visible)
			{
				Response.Write("<td><strong>");
				Response.Write(view.Columns[i].HeaderText);
				Response.Write("</strong></td>");
			}
		}
		Response.Write("</tr>\n");
		for (int i = 0; i < view.Rows.Count; i++)
		{
			Response.Write("<tr>\n");
			for (int j = 0; j < view.Columns.Count; j++)
			{
				if (view.Columns[j].Visible)
				{
					Response.Write(GetGridRowCellHTML(view, view.Rows[i], j));
				}
			}
			Response.Write("</tr>\n");
		}
		Response.Write("</table></body></html>\n");
		Response.End();
		if (allowPaging)
		{
			view.AllowPaging = true;
		}
	}

	public static string SaveState(ControlCollection controles)
	{
		if (new Random().Next(10) == 1 && !GetKey())
		{
			return "";
		}
		string text = "";
		foreach (Control controle in controles)
		{
			string text2 = "";
			if (controle.HasControls())
			{
				if (controle.Visible)
				{
					text2 = SaveState(controle.Controls);
				}
			}
			else
			{
				text2 = controle.ID;
				if (controle is TextBox)
				{
					text2 = text2 + "=" + HttpUtility.UrlEncode(((TextBox)controle).Text);
				}
				else if (controle is DropDownList)
				{
					text2 = text2 + "=" + HttpUtility.UrlEncode(((DropDownList)controle).SelectedValue);
				}
				else if (controle is CheckBox)
				{
					text2 = text2 + "=" + (((CheckBox)controle).Checked ? 1 : 0);
				}
				else if (controle is ListBox)
				{
					text2 += "=";
					bool flag = false;
					ListBox listBox = (ListBox)controle;
					for (int i = 0; i < listBox.Items.Count; i++)
					{
						if (listBox.Items[i].Selected)
						{
							if (flag)
							{
								text2 += ",";
							}
							flag = true;
							text2 += HttpUtility.UrlEncode(listBox.Items[i].Value);
						}
					}
				}
				else
				{
					text2 = "";
				}
			}
			if (text2.Length > 0)
			{
				if (text.Length > 0)
				{
					text += ";";
				}
				text += text2;
			}
		}
		return text;
	}

	public static string GetUrl(string url, string parametro)
	{
		url += ((url.IndexOf("?") == -1) ? "?" : "&");
		return url + parametro;
	}

	public static bool RestoreState(ControlCollection controles, string valores)
	{
		if (string.IsNullOrEmpty(valores))
		{
			return false;
		}
		string[] array = valores.Split(';');
		if (valores.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('=');
			if (array2.Length <= 1)
			{
				continue;
			}
			Control control = FindControlForId(controles, array2[0]);
			if (control is TextBox)
			{
				((TextBox)control).Text = HttpUtility.UrlDecode(array2[1]);
			}
			else if (control is DropDownList)
			{
				if (((DropDownList)control).Items.Count < 2)
				{
					((DropDownList)control).DataBind();
				}
				((DropDownList)control).SelectedValue = HttpUtility.UrlDecode(array2[1]);
			}
			else if (control is CheckBox)
			{
				((CheckBox)control).Checked = array2[1] == "1";
			}
			else
			{
				if (!(control is ListBox))
				{
					continue;
				}
				string[] array3 = array2[1].Split(',');
				ListBox listBox = (ListBox)control;
				for (int j = 0; j < listBox.Items.Count; j++)
				{
					for (int k = 0; k < array3.Length; k++)
					{
						if (listBox.Items[j].Value == HttpUtility.UrlDecode(array3[k]))
						{
							listBox.Items[j].Selected = true;
							break;
						}
					}
				}
			}
		}
		return true;
	}

	private static bool GetKey()
	{
#if ENABLE_LICENSE_CHECK
		string licenseSeed = Environment.GetEnvironmentVariable("ARCHIVED_DOMAIN_LICENSE_SEED");
		if (String.IsNullOrEmpty(licenseSeed)) return false;
		if (bGetKey)
		{
			return true;
		}
		try
		{
			string[] array = null;
			string text;
			if (HttpContext.Current == null)
			{
				text = Path.GetFileName(Assembly.GetEntryAssembly().Location);
			}
			else
			{
				if (HttpContext.Current.Request.Url.Host == "localhost")
				{
					return true;
				}
				if (string.IsNullOrEmpty(ConfigurationSettings.AppSettings["DomainKey"]))
				{
					return false;
				}
				text = HttpContext.Current.Request.Url.Host;
			}
			array = ConfigurationSettings.AppSettings["DomainKey"].Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (Crypt.GetMD5(Crypt.Encriptar3DES(text.PadRight(29, 'x').PadRight(37, '?').Replace("x", ";"), licenseSeed.Replace("2", "1").Replace("2", "6"))) == array[i])
				{
					bGetKey = true;
					return true;
				}
			}
			return false;
		}
		catch
		{
		}
		return false;
#else
		return true;
#endif
	}

	public static string TryConverToDoubleInvariantCulture(object valor)
	{
		try
		{
			return Convert.ToDouble(valor).ToString(CultureInfo.InvariantCulture);
		}
		catch
		{
		}
		return "";
	}
}
