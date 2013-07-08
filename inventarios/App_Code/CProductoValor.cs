using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDG.Componentes;

[Serializable]
public class CProductoValor
{
    protected String id;

    protected bool bDefaltCosto = false;
    protected bool bDefaltValor = false;
    protected bool bDefaltValorTemp = false;
    protected Decimal costo = 0;
    protected Decimal valor = 0;
    protected Decimal valor_temp = 0;

    protected Dictionary<int, Decimal> pCostos = new Dictionary<int, Decimal>();
    protected Dictionary<int, Decimal> pValorCliente = new Dictionary<int, Decimal>();
    protected Dictionary<int, Decimal> pValorAlmancen = new Dictionary<int, Decimal>();

	public CProductoValor(string idproducto)
	{
        id = idproducto;
	}

    public static CProductoValor GetProducto(string id) {

        CProductoValor info = (CProductoValor)Cache.Get(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "pvc" + id);
        if ( info!= null)
            return info;

        info = new CProductoValor(id);

        return info;
    }

    public static void Delete(string id) {
        Cache.Remove(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "pvc" + id);
    }

    public void Save() {
        Cache.Set(((CUsuario)Usuario.GetUsuario()).GetDatabase() + "pvc" + id, this);
    }

    public Decimal GetValorTemp() {
        
        return valor_temp;
    }
    
    public Decimal GetCosto()
    {
        return costo;
    }

    public Decimal GetValor(string cliente, int almacen) {

        using (CConector conector = new CConector())
        {
            if (!bDefaltValor)
            {
                bDefaltValor = true;
                valor = 0;
                try
                {
                    valor = Convert.ToDecimal(conector.QueryString(string.Format("SELECT valor FROM producto WHERE id={0}", id)));
                }
                catch { }
                Save();
            }
            if (valor > 0 && !bDefaltValorTemp)
            {
                bDefaltValorTemp = true;
                valor_temp = 0;
                try
                {
                    valor_temp = Convert.ToDecimal(conector.QueryString(string.Format("SELECT valor_temporal FROM producto WHERE id={0}", id)));
                }
                catch { }
                Save();
            }

            if (!string.IsNullOrEmpty(cliente))
            {
                try
                {
                    if (pValorCliente[Convert.ToInt32(cliente)] > 0)
                        return pValorCliente[Convert.ToInt32(cliente)];
                }
                catch { }

                try
                {
                    Decimal dValor = 0;
                    dValor = Convert.ToDecimal(conector.QueryString(string.Format("SELECT valor FROM valor_cliente WHERE producto={1} AND cliente={0}", cliente, id)));
                    pValorCliente[Convert.ToInt32(cliente)] = dValor;
                    Save();
                    if (dValor > 0)
                        return dValor;
                }
                catch { }
            }

            try
            {
                if (pValorAlmancen[almacen] > 0)
                    return pValorAlmancen[almacen];
            }
            catch { }

            try
            {
                Decimal dValor = 0;
                dValor = Convert.ToDecimal(conector.QueryString(string.Format("SELECT valor FROM valor_almacen WHERE producto={1} AND almacen={0}", almacen, id)));
                pValorAlmancen[almacen] = dValor;
                Save();
                if (dValor > 0)
                    return dValor;
            }
            catch { }
        }

        return valor > 0 ? valor : valor_temp;
    }

    public void UpdateValorTemp(Decimal nuevo_valor)
    {
        valor_temp = nuevo_valor;
        Save();
    }
    public void UpdateCostos(string proveedor, Decimal nuevo_costo) {
        
        costo = nuevo_costo;
        try
        {
            pCostos[Convert.ToInt32(proveedor)] = costo;
        } catch {}
        Save();
    }

    public Decimal GetCosto(string proveedor)
    {
        using (CConector conector = new CConector())
        {

            if (!bDefaltCosto)
            {
                bDefaltCosto = true;
                costo = 0;
                try
                {
                    costo = Convert.ToDecimal(conector.QueryString(string.Format("SELECT costo FROM producto WHERE id={0}", id)));
                }
                catch { }
                Save();
            }

            if (!string.IsNullOrEmpty(proveedor))
            {
                try
                {
                    return pCostos[Convert.ToInt32(proveedor)];
                }
                catch { }

                try
                {
                    Decimal dCosto = 0;
                    dCosto = Convert.ToDecimal(conector.QueryString(string.Format("SELECT costo FROM costo_proveedor WHERE producto={1} AND proveedor={0}", proveedor, id)));
                    pCostos[Convert.ToInt32(proveedor)] = dCosto;
                    Save();
                    return dCosto;
                }
                catch { }
            }
        }
        
        return costo;
    }
}
