<%@ Page Title="Página no encontrada" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="404.aspx.cs" Inherits="error_404" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <p align="center">
        Error<br />
        <span class="usuario" style="color: #001736; font-size: 120px">404</span>
        <br />
        Página no encontrada.<br />
        Intente con otro vinculo.<br />
        <a href="javascript:history.back();">Volver</a></p>
</asp:Content>

