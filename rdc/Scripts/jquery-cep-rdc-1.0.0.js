//Criado por Thiago para requisitar informações ao sistema rdc referentes ao cep e retornar ao HTML
$(function () {
    $('#buscacep').click(function () {
        var name = $('#CEP').val();
        this.href = this.href + '?name=' + encodeURIComponent(name);
    });
});
//Função para ao precionar o botão de Pesquisa da tela, o mesmo solicitar os dados do CEP. 
//Igual a de baixo porém o campo tem o nome "cep"(minúsculo)
function findcep() {
    var id = $('#cep').val();
    $.getJSON("GetCep/" + id,
    function (data) {
        var str = "Endereço do CEP : " + data.cep + " encontrado e preenchido nos campos abaixo : ";
        $('#produto').html(str);
        $('#cep').val(data.cep);
        $('#endereco').val(data.endereco);
        $('#ESTADO').val(data.estado);
        listaCidade(data.estado, data.cidade);
    })
    .fail(
        function (jqXHR, textStatus, err) {
            $('#produto').html('Erro : ' + err);
        });
}
//Função para ao precionar o botão de Pesquisa da tela, o mesmo solicitar os dados do CEP
function find() {
    var id = $('#CEP').val();
    $.getJSON("GetCep/" + id,
    function (data) {
        var str = "Endereço do CEP : " + data.cep + " encontrado e preenchido nos campos abaixo : ";
        $('#produto').html(str);
        $('#CEP').val(data.cep);
        $('#ENDERECO').val(data.endereco);
        $('#ESTADO').val(data.estado);
        listaCidade(data.estado,data.cidade);
    })
    .fail(
        function (jqXHR, textStatus, err) {
            $('#produto').html('Erro : ' + err);
        });
}
//Necessário para fazer a chamada Ajax via jQuery para preencher o 
$(document).ready(function () {
    $("#ESTADO").change(function () {
        listaCidade($(this).val());
    });
});
//chamada ajax para a Action ListaCidade         
//passando como parâmetro a Estado selecionado 
function listaCidade(uf, cd) {
    var itemcid = cd;
    $.getJSON("ListaCidade/" + uf, listaCidadeCallBackItem(itemcid));
}
//função que irá ser chamada quando terminar         
//a chamada ajax com passagem de um parâmetro externo idcidade(itemcid) para setar o combobox
function listaCidadeCallBackItem(itemcid) {
    return function (json) {
        //Limpar os itens que são maiores que 0             
        //Ou seja: não retirar o primeiro item
        $("#IDCIDADE :gt(0)").remove();
        $(json).each(function () {
            //adicionando as opções de acordo com o retorno
            if (this.Id == itemcid) {
                $("#IDCIDADE").append("<option value='" + this.Id + "' selected>" + this.Nome + "</option>");
            }
            else {
                $("#IDCIDADE").append("<option value='" + this.Id + "'>" + this.Nome + "</option>");
            }
        });
    }
}

//função que irá ser chamada quando terminar         
//a chamada ajax - desativada por enquanto
function listaCidadeCallBack(json) {
    //Limpar os itens que são maiores que 0             
    //Ou seja: não retirar o primeiro item
    $("#IDCIDADE :gt(0)").remove();
    $(json).each(function () {
        //adicionando as opções de acordo com o retorno
        $("#IDCIDADE").append("<option value='" + this.Id + "'>" + this.Nome + "</option>");
    });
}