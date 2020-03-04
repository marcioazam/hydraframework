using HydraFramework.Modules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using static HydraFramework.Modules.Manipula;

namespace HydraFramework
{
    public class Hydra
    {
        private Type Tipo;
        private readonly BaseHydra BaseHydra;

        public Hydra(DataContext conexao)
        {
            BaseHydra = new BaseHydra(conexao);
        }

        /// <summary>Salva um objeto no banco de dados.<br></br>Retorna o objeto salvo do banco de dados.</summary>
        /// <param name="entidade">Objeto a ser salvo</param>
        public object Save(object entidade)
        {
            entidade = BaseHydra.Save(entidade);

            return entidade;
        }

        /// <summary>Exclui um objeto no banco de dados.<br></br>Caso seja realizado com sucesso é retornado um boolean com valor true.</summary>
        /// <param name="entidade">Objeto a ser excluído</param>
        public object Delete(object entidade)
        {
            entidade = BaseHydra.Delete(entidade);

            return entidade;
        }

        /// <summary>Carrega uma tabela do banco de dados, convertendo a classe na tabela desejada. <br></br>Retorna a tabela convertida em uma lista da mesma classe.</summary>
        /// <param name="top">Define limite de linhas do Select</param>
        /// <param name="columns">Define colunas a serem selecionadas, usando uma ',' para serapar cada coluna</param>
        /// <param name="condition">Inclusão de condições extras, como por exemplo: WHERE, GROUP BY, HAVING e ORDER BY</param>
        /// <param name="parameters">Paramêtros a serem usados na consulta</param>
        public List<T> Load<T>(int? top = null, string columns = "*", string condition = "", HydraParameters parameters = null) 
        {
            string comandoSQL;
            Tipo = typeof(T);

            Manipula.Consulta(out comandoSQL, Tipo, TipoConsulta.Select, top, columns, condicoes: condition);

            var lista = BaseHydra.ConsultaLista<T>(comandoSQL, columns, parameters);

            return lista;
        }

       

        /// <summary>Carrega uma tabela do banco de dados, convertendo a classe na tabela desejada. <br></br>Retorna a tabela convertida em uma entidade da mesma classe.</summary>
        /// <param name="top">Define limite de linhas do Select</param>
        /// <param name="columns">Define colunas a serem selecionadas</param>
        /// <param name="condition">Inclusão de condições extras, como por exemplo: WHERE, GROUP BY, HAVING e ORDER BY</param>
        public T LoadSingle<T>( string columns = "*", string condition = "", HydraParameters parameters = null) where T : new()
        {
            string comandoSQL;
            Tipo = typeof(T);

            Manipula.Consulta(out comandoSQL, Tipo, TipoConsulta.Select, 1, columns, condicoes: condition);

            var entidade = BaseHydra.ConsultaEntidade<T>(comandoSQL, columns, parameters);

            return entidade;
        }

        /// <summary>Executa uma consulta personalizada no banco de dados.<br></br>Caso seja realizado com sucesso é retornado um boolean com valor true.</summary>
        /// <param name="queryString">String da consulta a ser feita</param>
        /// <param name="parameters">Paramêtros a serem usados na consulta</param>
        public bool ExecuteQuery(string queryString, HydraParameters parametros = null)
        {
            bool resultado = BaseHydra.Consulta(queryString, parametros);

            return resultado;
        }

        /// <summary>Executa uma consulta personalizada no banco de dados. <br></br>Retorna a consulta convertida em uma lista da classe designada.</summary>
        /// <param name="queryString">String da consulta a ser feita</param>
        /// <param name="parameters">Paramêtros a serem usados na consulta</param>
        public List<T> ReturnQuery<T>(string queryString, HydraParameters parameters = null)
        {
            DataTable dataTable = BaseHydra.ConsultaEmDataTable(queryString, parameters);

            var lista = BaseHydra.ConverteDataTableEmLista<T>(dataTable);

            return lista;
        }

        /// <summary>Executa uma consulta personalizada no banco de dados. <br></br>Retorna a consulta convertida em uma lista da classe designada.</summary>
        /// <param name="queryString">String da consulta a ser feita</param>
        /// <param name="parameters">Paramêtros a serem usados na consulta</param>
        public T ReturnQuerySingle<T>(string queryString, HydraParameters parameters = null) 
        {
            DataTable dataTable = BaseHydra.ConsultaEmDataTable(queryString, parameters);

            var entidade = BaseHydra.ConverteDataTableEmEntidade<T>(dataTable);

            return entidade;
        }

        /// <summary>Executa uma consulta personalizada no banco de dados. <br></br>Retorna a consulta convertida em uma tupla de 2 itens</summary>
        /// <param name="queryString">String da consulta a ser feita</param>
        /// <param name="parameters">Paramêtros a serem usados na consulta</param>
        public List<HydraTuple<T1, T2>> ReturnQuery<T1, T2>(string queryString, HydraParameters parameters = null) 
        {
            var lista = BaseHydra.ConsultaLista<T1, T2>(queryString, parameters);

            return lista;
        }

        /// <summary>Executa uma consulta personalizada no banco de dados. <br></br>Retorna a consulta convertida em uma tupla de 3 itens</summary>
        /// <param name="queryString">String da consulta a ser feita</param>
        /// <param name="parameters">Paramêtros a serem usados na consulta</param>
        public List<HydraTuple<T1, T2, T3>> ReturnQuery<T1, T2, T3>(string queryString, HydraParameters parameters = null)
        {
            var lista = BaseHydra.ConsultaLista<T1, T2, T3>(queryString, parameters);

            return lista;
        }

        /// <summary>Carrega uma tabela do banco de dados, convertendo a classe na tabela desejada. <br></br>Retorna um DataTable.</summary>
        /// <param name="top">Define limite de linhas do Select</param>
        /// <param name="columns">Define colunas a serem selecionadas</param>
        /// <param name="condition">Inclusão de condições extras, como por exemplo: WHERE, GROUP BY, HAVING e ORDER BY</param>
        public DataTable LoadFromDataTable(string queryString, HydraParameters parameters = null)
        {
            var dataTable = BaseHydra.ConsultaEmDataTable(queryString, parameters);

            return dataTable;
        } 
    }
}