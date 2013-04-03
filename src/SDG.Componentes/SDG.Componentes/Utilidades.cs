using System;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SDG.Componentes;

public class Utilidades
{
	public static string GetURLFullPath(Page page, string archivo)
	{
		string text = page.Request.Url.Host;
		if (page.Request.Url.Port != 80)
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
		return RenderGrid(grid, "border=0 width=100% cellspacing=0");
	}

	public static string RenderGrid(GridView grid, string parametrostabla)
	{
		if (grid.Rows.Count == 0)
		{
			return grid.EmptyDataText;
		}
		string text = "<table " + parametrostabla + ">";
		text += "<tr>";
		for (int i = 0; i < grid.Columns.Count; i++)
		{
			if (grid.Columns[i].Visible)
			{
				text = text + "<td><b>" + grid.Columns[i].HeaderText + "</b></td>";
			}
		}
		text += "</tr>";
		for (int i = 0; i < grid.Rows.Count; i++)
		{
			GridViewRow gridViewRow = grid.Rows[i];
			text += "<tr>";
			for (int j = 0; j < gridViewRow.Cells.Count; j++)
			{
				if (!grid.Columns[j].Visible)
				{
					continue;
				}
				text = ((grid.Columns[j].ItemStyle.HorizontalAlign == HorizontalAlign.Right) ? (text + "<td align=right>") : ((grid.Columns[j].ItemStyle.HorizontalAlign != HorizontalAlign.Center) ? (text + "<td>") : (text + "<td align=center>")));
				if (gridViewRow.Cells[j].HasControls())
				{
					ControlCollection controls = gridViewRow.Cells[j].Controls;
					for (int k = 0; k < controls.Count; k++)
					{
						if (controls[k] is TextBox)
						{
							string text2 = ((TextBox)controls[k]).Text;
							if (text2.Length == 0)
							{
								text2 = "&nbsp;";
							}
							text += text2;
							break;
						}
						if (controls[k] is CheckBox)
						{
							text = ((!((CheckBox)controls[k]).Checked) ? (text + "_ ") : (text + "X "));
							if (((CheckBox)controls[k]).Text.Length > 0)
							{
								text += ((CheckBox)controls[k]).Text;
							}
							break;
						}
						if (controls[k] is LinkButton)
						{
							text += ((LinkButton)controls[k]).Text;
						}
						else if (controls[k] is HyperLink)
						{
							text += ((HyperLink)controls[k]).Text;
						}
					}
				}
				else
				{
					text += gridViewRow.Cells[j].Text;
				}
				text += "</td>";
			}
			text += "</tr>";
		}
		return text + "</table>";
	}

	public static string ReplaceContenido(ControlCollection controles, string contenido)
	{
		for (int i = 0; i < controles.Count; i++)
		{
			if (!controles[i].Visible)
			{
				continue;
			}
			string iD = controles[i].ID;
			string newValue = "";
			if (controles[i].Controls.Count > 0 && controles[i].GetType() != typeof(GridView) && controles[i].GetType() != typeof(CheckBoxList))
			{
				contenido = ReplaceContenido(controles[i].Controls, contenido);
			}
			else
			{
				if (iD == null || iD.Length == 0)
				{
					continue;
				}
				iD = "$" + controles[i].ID + "$";
				if (controles[i].GetType() == typeof(TextBox))
				{
					newValue = ((TextBox)controles[i]).Text;
				}
				else if (controles[i].GetType() == typeof(CheckBox))
				{
					newValue = (((CheckBox)controles[i]).Checked ? "X" : "_");
				}
				else if (controles[i].GetType() == typeof(Label))
				{
					newValue = ((Label)controles[i]).Text;
				}
				else if (controles[i].GetType() == typeof(DropDownList))
				{
					if (((DropDownList)controles[i]).SelectedItem == null)
					{
						((DropDownList)controles[i]).DataBind();
					}
					newValue = ((DropDownList)controles[i]).SelectedItem.Text;
				}
				else if (controles[i].GetType() == typeof(HiddenField))
				{
					newValue = ((HiddenField)controles[i]).Value;
				}
				else if (controles[i].GetType() == typeof(GridView))
				{
					newValue = RenderGrid((GridView)controles[i]);
				}
				else if (controles[i].GetType() == typeof(CheckBoxList))
				{
					newValue = RenderCheckBoxList((CheckBoxList)controles[i]);
				}
				if (contenido.IndexOf(iD) != -1)
				{
					contenido = contenido.Replace(iD, newValue);
				}
			}
		}
		return contenido;
	}

	public static StateBag GetViewState()
	{
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
		bool allowPaging = view.AllowPaging;
		if (allowPaging)
		{
			view.AllowPaging = false;
			view.DataBind();
		}
		Response.Clear();
		Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(titulo + ".xls"));
		Response.ContentType = "application/vnd.ms-excel";
		Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
		Response.Write("<html>");
		Response.Write("<body>\n");
		Response.Write("<table border='1'>\n");
		Response.Write("<tr>\n");
		for (int i = 0; i < view.Columns.Count; i++)
		{
			Response.Write("<td><strong>");
			Response.Write(view.Columns[i].HeaderText);
			Response.Write("</strong></td>");
		}
		Response.Write("</tr>\n");
		for (int i = 0; i < view.Rows.Count; i++)
		{
			Response.Write("<tr>\n");
			for (int j = 0; j < view.Columns.Count; j++)
			{
				Response.Write("<td>");
				if (view.Columns[j].GetType() == typeof(HyperLinkField))
				{
					HyperLink hyperLink = (HyperLink)view.Rows[i].Cells[j].Controls[0];
					Response.Write(hyperLink.Text);
				}
				else if (view.Columns[j].GetType() == typeof(CheckBoxField))
				{
					CheckBox checkBox = (CheckBox)view.Rows[i].Cells[j].Controls[0];
					Response.Write(checkBox.Checked ? "X" : "");
				}
				else
				{
					Response.Write(view.Rows[i].Cells[j].Text);
				}
				Response.Write("</td>");
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
}
