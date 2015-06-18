using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Permisos
/// </summary>
public class Permisos
{
    protected string _Pagina;
    protected string _Titulo;
    protected int permisos;

    public Permisos(string tit, string pag)
    {

        _Titulo = tit;
        _Pagina = pag;
        permisos = 0;
    }
    public Permisos(string tit, string pag, int per)
    {
        _Titulo = tit;
        _Pagina = pag;
        permisos = per;
    }
    public string titulo
    {
        get { return _Titulo; }
    }
    public string pagina
    {
        get { return _Pagina; }
    }
    public void SetPermisos(int per)
    {
        permisos = per;
    }
    public bool administrador
    {
        get { return (permisos & 32) == 32; }
    }
    public bool eliminar
    {
        get { return (permisos & 16) == 16; }
    }
    public bool edicion
    {
        get { return (permisos & 8) == 8; }
    }
    public bool insercion
    {
        get { return (permisos & 4) == 4; }
    }
    public bool imprimir
    {
        get { return (permisos & 2) == 2; }
    }
    public bool lectura
    {
        get { return (permisos & 1) == 1; }
    }
    public bool todos
    {
        get { return (permisos & 63) == 63; }
    }
};