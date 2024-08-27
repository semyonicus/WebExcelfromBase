<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="anketa.SurveyForm" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Выгрузка данных</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
       <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>
   
    <style>
        .survey-form {
            max-width: 600px;
            margin: 50px auto;
            padding: 30px;
            border: 1px solid #ddd;
            border-radius: 10px;
            background-color: #f9f9f9;
        }
        .form-group {
            position: relative;
        }
        .help-button {
            display: inline-block;
            width: 24px;
            height: 24px;
            border-radius: 50%;
            background-color: #007bff;
            color: white;
            text-align: center;
            line-height: 24px;
            margin-left: 10px;
            cursor: pointer;
            transition: background-color 0.3s;
            right: 0;
            top: 50%;
           }
        .help-button:hover {
            background-color: #0056b3;
        }
        .popover {
            transition: opacity 0.3s;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $('[data-toggle="popover"]').popover({
                placement: 'bottom',
                delay: {
                    "show": 500,
                    "hide": 100
                }
            });
        });
    </script>
</head>
<body>
    <div class="container">
        <form id="form1" runat="server" class="survey-form" enctype="multipart/form-data">
            
            <h2 class="text-center">
                Выгрузка данных</h2>
            <div class="form-group">
                <span class="help-button" data-toggle="popover" 
                    data-content="Конфигурация представляет собой
                    сценарии выполненные на хранимых процедурах 
                    которые обращаются к базе данных. 
                    В отличии от предыдущих систем здесь вы можете 
                    менять параметры хранимых процедур, 
                    когда все придет в окончательный вид, 
                    параметры надеюсь будут продокументированы">?
                </span>
            Выберите конфигурацию <br/>
         
                <asp:ListBox ID="ListBoxConfigs" runat="server" 
                AutoPostBack="true" 
                OnSelectedIndexChanged="ListBoxConfigs_SelectedIndexChanged"
                CssClass="form-control" style="width: 100%;"
                ></asp:ListBox>
       
                <asp:TextBox ID="TemplateName" runat="server" CssClass="form-control" style="width: 100%;"></asp:TextBox>
            <span class="help-button" 
         data-toggle="popover" data-content="Выгрузка данных 
         может редактироваться прямо здесь, 
         все зависит от выбора конфигурации">?</span>
      
                <asp:TextBox ID="QueryArea" runat="server" 
                TextMode="MultiLine" Rows="10" Columns="50"
                CssClass="form-control" style="width: 100%;"
                ></asp:TextBox>
                <asp:Label runat="server" ID="desc"></asp:Label>
             </div>
            
            <asp:Button ID="SubmitButton" runat="server" Text="Загрузить данные" CssClass="btn btn-primary btn-block" OnClick="SubmitButton_Click" />
        
               </form>

    </div>
</body>
</html>
