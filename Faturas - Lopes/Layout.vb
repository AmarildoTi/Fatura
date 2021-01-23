Imports Marpress.Interfaces
Imports Marpress.Padrao.Fatura
Imports Marpress.Padrao
Imports Marpress.Funcoes

Public Class Layout
    Inherits Fatura


    Private _textoimpressao As String
    Public Property TextoImpressao() As String
        Get
            Return _textoimpressao
        End Get
        Set(ByVal value As String)
            _textoimpressao = value
        End Set
    End Property

    Private _descricaomensagem As String
    Public Property DescricaoMensagem() As String
        Get
            Return _descricaomensagem
        End Get
        Set(ByVal value As String)
            _descricaomensagem = value
        End Set
    End Property

    Private _xmen_01_09 As String
    Public Property XMen_01_09() As String
        Get
            Return _xmen_01_09
        End Get
        Set(ByVal value As String)
            _xmen_01_09 = value
        End Set
    End Property

    Private _xmen_02_09 As String
    Public Property XMen_02_09() As String
        Get
            Return _xmen_02_09
        End Get
        Set(ByVal value As String)
            _xmen_02_09 = value
        End Set
    End Property

    Private _qtdeparcela As String
    Public Property QtdeParcela() As String
        Get
            Return _qtdeparcela
        End Get
        Set(ByVal value As String)
            _qtdeparcela = value
        End Set
    End Property

    Private _valorparcela As String
    Public Property ValorParcela() As String
        Get
            Return _valorparcela
        End Get
        Set(ByVal value As String)
            _valorparcela = value
        End Set
    End Property

    Private _taxamensal As String
    Public Property TaxaMensal() As String
        Get
            Return _taxamensal
        End Get
        Set(ByVal value As String)
            _taxamensal = value
        End Set
    End Property

    Private _sinal As String
    Public Property Sinal() As String
        Get
            Return _sinal
        End Get
        Set(ByVal value As String)
            _sinal = value
        End Set
    End Property

    Private _idarquivo As String
    Public Property IDArquivo() As String
        Get
            Return _idarquivo
        End Get
        Set(ByVal value As String)
            _idarquivo = value
        End Set
    End Property

    Private _idproduto As String
    Public Property IDProduto() As String
        Get
            Return _idproduto
        End Get
        Set(ByVal value As String)
            _idproduto = value
        End Set
    End Property

    Private _produto As String
    Public Property Produto() As String
        Get
            Return _produto
        End Get
        Set(ByVal value As String)
            _produto = value
        End Set
    End Property

    Private _grupoafinidade As String
    Public Property GrupoAfinidade() As String
        Get
            Return _grupoafinidade
        End Get
        Set(ByVal value As String)
            _grupoafinidade = value
        End Set
    End Property

    Private _modelofatura As ModeloFatura
    Public Property ModeloFatura() As ModeloFatura
        Get
            Return _modelofatura
        End Get
        Set(ByVal value As ModeloFatura)
            _modelofatura = value
        End Set
    End Property

    Private _idprocessamento As String
    Public Property IDProcessamento() As String
        Get
            Return _idprocessamento
        End Get
        Set(ByVal value As String)
            _idprocessamento = value
        End Set
    End Property

    Public Sub CarregaRegistros(ByVal linha As String)
        Try
            With linha
                XMen_01_09 = "AQUI DEVERIA ESTAR SEU NÚMERO DA SORTE. FAÇA A ADESÃO AO NOVO SEGURO DESEMPREGO PREMIÁVEL"
                XMen_02_09 = "            E PASSE A CONCORRER A PRÊMIOS DE R$ 3.750,00 QUATRO VEZES POR MÊS.           "
                Select Case .Substring(0, 2)
                    Case "00"
                        CIF.TipoRegistro = "Lopes"
                        Remetente.Apelido = "Lopes"
                        Remetente.Nome = "Lopes Supermecados"
                        Remetente.Endereco.Logradouro = "Av. São Paulo"
                        Remetente.Endereco.Numero = "355"
                        Remetente.Endereco.Bairro = "Jardim Tranquilidade"
                        Remetente.Endereco.Cidade = "Guarulhos"
                        Remetente.Endereco.Estado = "SP"
                        Remetente.Endereco.CEP = "07052160"
                    Case "01"
                        Destinatario.Nome = linha.Substring(14, 40)
                        Cartoes.Add(New Cartao)
                        With Cartoes(Cartoes.Count - 1)
                            .Numero = linha.Substring(54, 19)
                        End With
                        Destinatario.Documento = .Substring(2, 12)
                        Destinatario.Endereco.Logradouro = .Substring(73, 72)
                        Destinatario.Endereco.Numero = .Substring(145, 10)
                        Destinatario.Endereco.Complemento = .Substring(155, 72)
                        Destinatario.Endereco.Bairro = .Substring(227, 72)
                        Destinatario.Endereco.Cidade = .Substring(308, 72)
                        Destinatario.Endereco.Estado = .Substring(380, 2).Trim
                        Destinatario.Endereco.CEP = .Substring(299, 9)
                    Case "02"
                        Cartoes(Cartoes.Count - 1).Despesa.Add(New Despesa)
                        With Cartoes(Cartoes.Count - 1).Despesa(Cartoes(Cartoes.Count - 1).Despesa.Count - 1)
                            .Data = linha.Substring(17, 10)
                            .Loja = linha.Substring(27, 15)
                            .Portador = linha.Substring(70, 4)
                            .NumeroOperacao = linha.Substring(75, 7)
                            .Caixa = linha.Substring(82, 3)
                            .Descricao = linha.Substring(85, 32)
                            .Valor = linha.Substring(117, 15)
                        End With
                    Case "03"
                        Vencimento = .Substring(14, 10)
                        TotalFatura = .Substring(110, 18)
                        PagamentoMinimo = .Substring(24, 18)
                        LimiteCredito = .Substring(89, 18)
                        ProximosLancamentos = .Substring(128, 18)
                        EncargosProximoMes = .Substring(202, 6)
                        EncargosMes = .Substring(208, 6)
                        TextoImpressao = linha.Substring(827, 65)
                    Case "04"
                        DescricaoMensagem = linha.Substring(44, 260)
                    Case "05"
                        Boleto.Parcelas.Add(New Parcela)
                        Boleto.Parcelas(0).LinhaDigitavel = .Substring(14, 59).Trim
                        Boleto.LocalDePagamento = .Substring(73, 63)
                        Boleto.Parcelas(0).Vencimento = .Substring(136, 20).Trim
                        Boleto.Beneficiario.Nome = .Substring(156, 63).Trim()
                        Boleto.Beneficiario.Endereco.Logradouro = .Substring(810, 70).Trim()
                        Boleto.AgenciaCodigoBeneficiario = .Substring(219, 16)
                        Boleto.DataProcessamento = .Substring(249, 10).Trim
                        Boleto.Parcelas(0).NumeroDocumento = .Substring(772, 12)
                        Boleto.EspecieDocumento = .Substring(245, 3)
                        Boleto.Aceite = .Substring(248, 1)
                        Boleto.Especie = .Substring(807, 4)
                        Boleto.Parcelas(0).NossoNumero = .Substring(259, 20)
                        Boleto.Parcelas(0).Valor = .Substring(785, 11)
                        Boleto.Parcelas(0).Instrucoes.Add(.Substring(350, 57))
                        Boleto.Parcelas(0).Instrucoes.Add(.Substring(407, 57))
                        Boleto.Parcelas(0).Instrucoes.Add(.Substring(464, 57))
                        Boleto.Parcelas(0).Instrucoes.Add(.Substring(521, 57))
                        Boleto.Parcelas(0).Instrucoes.Add(.Substring(578, 57))
                        Boleto.Parcelas(0).Instrucoes.Add(.Substring(635, 57))
                        Boleto.Parcelas(0).Instrucoes.Add(.Substring(692, 57))
                        Boleto.Parcelas(0).CodigoDeBarra = .Substring(306, 57)
                    Case "09"
                        XMen_01_09 = "ESTE É SEU NÚMERO DA SORTE:" & linha.Substring(51, 5).Trim & ".COM ELE VOCÊ CONCORRE A PRÊMIOS PELA LOTERIA FEDERAL."
                        XMen_02_09 = "É NECESSÁRIO ESTAR EM DIA COM O PAGAMENTO DE SUA FATURA."
                    Case "10"
                        QtdeParcela = linha.Substring(17, 3)
                        ValorParcela = linha.Substring(38, 17)
                        TaxaMensal = linha.Substring(56, 7)
                        Sinal = linha.Substring(63, 1)
                        Mensagem.Add(.Substring(112, 260))
                End Select
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Function MontarLinha() As String
        Dim linha As String = "01"
        'Try
        '    linha += _tipoFatura
        '    linha += _correio
        '    linha += _codigoAssociado
        '    linha += _destinario.Nome
        '    linha += _destinario.Endereco.Logradouro
        '    linha += _destinario.Endereco.Bairro
        '    linha += _destinario.Endereco.CEP
        '    linha += _destinario.Endereco.Cidade
        '    linha += _destinario.Endereco.Estado
        '    linha += _titulo
        '    linha += _dataemissao
        '    linha += _dataVencimento
        '    linha += _identificadortitulo
        '    linha += _banco
        '    linha += _agencia
        '    linha += _conta
        '    linha += _contadv
        '    linha += _carteira
        '    linha += _especie
        '    linha += _mora
        '    linha += _desconto
        '    linha += _datadesconto
        '    linha += _abatimento
        '    linha += _valorDocumento
        '    linha += _mensagem(0)
        '    linha += _mensagem(1)
        '    linha += _mensagem(2)
        '    linha += _mensagem(3)
        '    linha += _quantidade
        '    linha += _linha
        '    linha += _demonstrativo(0)
        '    linha += _destinario.Documento
        '    linha += _reservado
        '    linha += _sequencia
        'Catch ex As Exception
        'MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        'End Try
        Return linha
    End Function

End Class
