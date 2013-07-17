using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace rdc.Models
{
    public class EstadoRepositorio
    {
        public static ArrayList ListaEstados()
        {
            string SelectedEstado = "";
            //CRIA UMA LISTA DE ESTADOS E ATRIBUI A UM SELECT LIST
            ArrayList ListEstado = new ArrayList();
            ListEstado.Add(new TEstados("AC", "Acre", SelectedEstado));
            ListEstado.Add(new TEstados("AL", "Alagoas", SelectedEstado));
            ListEstado.Add(new TEstados("AP", "Amapá", SelectedEstado));
            ListEstado.Add(new TEstados("AM", "Amazonas", SelectedEstado));
            ListEstado.Add(new TEstados("BA", "Bahia", SelectedEstado));
            ListEstado.Add(new TEstados("CE", "Ceará", SelectedEstado));
            ListEstado.Add(new TEstados("DF", "Distrito Federal", SelectedEstado));
            ListEstado.Add(new TEstados("ES", "Espírito Santo", SelectedEstado));
            ListEstado.Add(new TEstados("GO", "Goiás", SelectedEstado));
            ListEstado.Add(new TEstados("MA", "Maranhão", SelectedEstado));
            ListEstado.Add(new TEstados("MT", "Mato Grosso", SelectedEstado));
            ListEstado.Add(new TEstados("MS", "Mato Grosso do Sul", SelectedEstado));
            ListEstado.Add(new TEstados("MG", "Minas Gerais", SelectedEstado));
            ListEstado.Add(new TEstados("PA", "Pará", SelectedEstado));
            ListEstado.Add(new TEstados("PB", "Paraíba", SelectedEstado));
            ListEstado.Add(new TEstados("PR", "Paraná", SelectedEstado));
            ListEstado.Add(new TEstados("PE", "Pernambuco", SelectedEstado));
            ListEstado.Add(new TEstados("PI", "Piauí", SelectedEstado));
            ListEstado.Add(new TEstados("RJ", "Rio de Janeiro", SelectedEstado));
            ListEstado.Add(new TEstados("RN", "Rio Grande do Norte", SelectedEstado));
            ListEstado.Add(new TEstados("RS", "Rio Grande do Sul", SelectedEstado));
            ListEstado.Add(new TEstados("RO", "Rondônia", SelectedEstado));
            ListEstado.Add(new TEstados("RR", "Roraima", SelectedEstado));
            ListEstado.Add(new TEstados("SC", "Santa Catarina", SelectedEstado));
            ListEstado.Add(new TEstados("SP", "São Paulo", SelectedEstado));
            ListEstado.Add(new TEstados("SE", "Sergipe", SelectedEstado));
            ListEstado.Add(new TEstados("TO", "Tocantins", SelectedEstado));

            return ListEstado;
        }
    }
}