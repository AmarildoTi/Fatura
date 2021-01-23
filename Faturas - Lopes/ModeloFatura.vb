Imports Marpress.Funcoes
Imports Marpress.FichaCompensacao

Public Class ModeloFatura

    Private _conteudoVerso As New List(Of String)
    Public Property ConteudoVerso() As List(Of String)
        Get
            Return _conteudoVerso
        End Get
        Set(ByVal value As List(Of String))
            _conteudoVerso = value
        End Set
    End Property

    Private _conteudoFrente As New List(Of String)
    Public Property ConteudoFrente() As List(Of String)
        Get
            Return _conteudoFrente
        End Get
        Set(ByVal value As List(Of String))
            _conteudoFrente = value
        End Set
    End Property

    Public Sub New(ByVal fatura As Layout)
        With fatura
            _conteudoFrente.Add("Arial|9|0|" & .Destinatario.Nome)
            _conteudoFrente.Add("Arial|9|0|" & .Cartoes(0).Numero.Trim)
            _conteudoFrente.Add("Arial|9|0|" & .Vencimento.Trim)
            _conteudoFrente.Add("Arial|9|0|" & .TotalFatura)
            _conteudoFrente.Add("Arial|9|0|" & .PagamentoMinimo)
            _conteudoFrente.Add("Arial|9|0|" & .LimiteCredito)
            _conteudoFrente.Add("Arial|9|0|" & .ProximosLancamentos)
            _conteudoFrente.Add("Arial|9|0|" & .EncargosMes & Space(1) & "%")
            _conteudoFrente.Add("Arial|9|0|" & .EncargosProximoMes & Space(1) & "%")
            _conteudoFrente.Add("Arial|9|0|" & .PagamentoFaturaAnterior)
            _conteudoFrente.Add("Arial|7|0|" & .XMen_01_09)
            _conteudoFrente.Add("Arial|7|0|" & .XMen_02_09)
            _conteudoFrente.Add("Arial|8|1|" & .TextoImpressao.Trim & Space(5) & .TotalFatura.Trim)
            _conteudoFrente.Add("Arial|7|0|" & .DescricaoMensagem)
            _conteudoFrente.Add("Arial|14|1|" & .QtdeParcela)
            _conteudoFrente.Add("Arial|14|1|" & .ValorParcela)
            _conteudoFrente.Add("Arial|14|1|" & .TaxaMensal & Space(1) & .Sinal)
            If .Mensagem.Count > 0 Then
                _conteudoFrente.Add("Arial|6|0|" & .Mensagem(0))
            Else
                _conteudoFrente.Add("Arial|6|0|")
            End If
            _conteudoFrente.Add("Arial|6|0|")
            _conteudoFrente.Add("Arial|12|1|" & .Boleto.Parcelas(0).LinhaDigitavel)
            _conteudoFrente.Add("Arial|9|1|" & .Boleto.LocalDePagamento)
            _conteudoFrente.Add("Arial|10|1|" & .Vencimento)
            _conteudoFrente.Add("Arial|8|0|" & .Boleto.Beneficiario.Nome & Space(5) & .Boleto.Beneficiario.Endereco.Logradouro)
            _conteudoFrente.Add("Arial|9|0|" & .Boleto.AgenciaCodigoBeneficiario)
            _conteudoFrente.Add("Arial|9|0|" & .Boleto.DataProcessamento)
            _conteudoFrente.Add("Arial|9|0|" & .Boleto.Parcelas(0).NumeroDocumento)
            _conteudoFrente.Add("Arial|9|0|" & .Boleto.Parcelas(0).NossoNumero)
            _conteudoFrente.Add("Arial|8|0|" & .Boleto.EspecieDocumento)
            _conteudoFrente.Add("Arial|8|0|" & .Boleto.Aceite)
            _conteudoFrente.Add("Arial|6|0|" & .Boleto.Carteira)
            _conteudoFrente.Add("Arial|9|0|" & .Boleto.Especie)
            _conteudoFrente.Add("Arial|9|1|" & EditaDois(.Boleto.Parcelas(0).Valor))

            If fatura.CIF.TipoRegistro = "A3C" Then
                _conteudoFrente.Add("Arial|8|0|" & Space(1) + "SR. CAIXA: NÃO ACEITAR CHEQUES PARA PAGAMENTO DA FATURA.")
                _conteudoFrente.Add("Arial|8|0|" & Space(1) + "PAGAMENTO NO BANCO: LIMITE LIBERADO APÓS PROCESSAMENTO.")
                _conteudoFrente.Add("Arial|8|0|" & Space(1) + "PAGUE QUALQUER VALOR ENTRE O MÍNIMO E O TOTAL DA FATURA.")
                _conteudoFrente.Add("Arial|8|0|" & Space(1) + "SOBRE A DIFERENÇA INCIDIRÃO ENCARGOS CONTRATUAIS.")
                _conteudoFrente.Add("Arial|8|0|" & Space(1) + "SE OPTAR PELO PARCELAMENTO, PAGUE O ""VALOR FIXO PARCELA"",")
                _conteudoFrente.Add("Arial|8|0|" & Space(1) + "CONSTANTE NO QUADRO DA OFERTA ESPECIAL PARA PARCELAMENTO.")
                _conteudoFrente.Add("Arial|8|0|" & Space(1) + "EM CASO DE DÚVIDAS,PROCURE O ATENDIMENTO DO CARTÃO LOPES.")
            Else
                For Each instrucao In .Boleto.Parcelas(0).Instrucoes
                    _conteudoFrente.Add("Arial|8|0|" & instrucao)
                Next
            End If

            _conteudoFrente.Add("Arial|8|0|" & .Destinatario.Nome)
            _conteudoFrente.Add("Arial|8|0|" & .Destinatario.Endereco.Logradouro.Trim & "  " & .Destinatario.Endereco.Bairro)
            _conteudoFrente.Add("Arial|8|0|" & SomenteNumeros(.Destinatario.Endereco.CEP).Insert(5, "-") & "  " & .Destinatario.Endereco.Cidade.Trim & "  " & .Destinatario.Endereco.Estado)
            _conteudoFrente.Add(.Boleto.Parcelas(0).CodigoDeBarra.Trim)

            For Each despesa In fatura.Cartoes(0).Despesa
                _conteudoFrente.Add("Courier New|8|0|" & despesa.Data.Trim.PadRight(10) & Space(1) & despesa.Loja.Trim.PadRight(15) & Space(4) & despesa.Portador.Trim.PadRight(4) & Space(5) & despesa.NumeroOperacao.Trim.PadRight(7) & Space(5) & despesa.Caixa.Trim.PadRight(3) & Space(4) & despesa.Descricao.Trim.PadRight(32) & Space(1) & despesa.Valor.Trim.PadLeft(14))
            Next

            ' Verso
            _conteudoVerso.Add("Arial|10|0|" & .Destinatario.Nome.Trim)
            _conteudoVerso.Add("Arial|10|0|" & .Destinatario.Endereco.Logradouro.Trim)
            _conteudoVerso.Add("Arial|10|0|" & .Destinatario.Endereco.Bairro.Trim)
            _conteudoVerso.Add("Arial|10|0|" & .Destinatario.Endereco.Cidade.Trim & "  " & .Destinatario.Endereco.Estado)
            _conteudoVerso.Add("Arial|10|0|" & SomenteNumeros(.Destinatario.Endereco.CEP).Insert(5, "-"))
            _conteudoVerso.Add("Arial|10|0|" & .CIF.CodigoCIF)
            _conteudoVerso.Add("Arial|6|0|" & .IDProcessamento)
        End With
    End Sub

End Class