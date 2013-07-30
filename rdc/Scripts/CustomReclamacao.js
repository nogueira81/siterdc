//Função para ao precionar o botão de Pesquisa da tela, o mesmo solicitar os dados do CEP
function listaFor() {
    var id = $('#FILTRAFORNEC').val();
    $.getJSON("listaFornec/" + id,
    function (data) {
        $("#IDFORNECEDOR :gt(0)").remove();
        $(data).each(function () {
            //adicionando as opções de acordo com o retorno
            $("#IDFORNECEDOR").append("<option value='" + this.IdFor + "'>" + this.NomeFor + "</option>");
        });
        $('#IDFORNECEDOR').focus();
    })
    .fail(
        function (jqXHR, textStatus, err) {
            $('#produto').html('Erro : ' + err);
        });
}