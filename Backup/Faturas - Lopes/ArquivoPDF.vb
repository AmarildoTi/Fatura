Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.pdf.parser
Imports iTextSharp.text.BaseColor
Imports Marpress.Funcoes

Public Class ArquivoPDF

    Public Shared Sub criar(ByVal diretorio As String, ByVal nomearquivo As String, ByVal faturas As ArquivoFatura, ByVal imagemfrente As String, ByVal imagemverso As String, ByVal label As Label)
        Try
            If Not Directory.Exists(diretorio) Then
                Directory.CreateDirectory(diretorio)
            End If
            For i As Integer = 1 To 3

                Dim processamento As String = ""

                If i = 1 Then
                    processamento = "A4"
                    imagemverso = "lopes_a4_v.pdf"
                    imagemfrente = "lopes_a4_f.pdf"
                ElseIf i = 2 Then
                    processamento = "A3C"
                    imagemverso = "lopes_a3c_v.pdf"
                    imagemfrente = "lopes_a3c_f.pdf"
                Else
                    processamento = "A3N"
                    imagemverso = "lopes_a3n_v.pdf"
                    imagemfrente = "lopes_a3n_f.pdf"
                End If


                Dim local As Integer = 0
                Dim estadual As Integer = 0
                Dim nacional As Integer = 0
                Dim cont As Integer = 0
                Dim cont2 As Integer = 0
                Dim nomearquivosaida As String = ""
                Dim lote As String = ""
                Dim arq As Integer = 1
                Dim texto As String = label.Text & vbCrLf
                Dim documento As Document
                Dim escritor As PdfWriter
                Dim codpostagem As String = ""
                Dim codadministrativo As String = ""

                For Each fatura In faturas.Linhas
                    If fatura.CIF.TipoRegistro = processamento Then
                        cont += 1
                        cont2 += 1
                        codpostagem = fatura.CIF.CodigoPostagem
                        codadministrativo = fatura.CIF.CodigoAdministrativo
                        If cont2 > 500 Then
                            cont2 = 1
                            arq += 1
                            documento.Close()
                            escritor.Close()
                            If cont > 1 Then ArquivoOS.Fatura(diretorio, nomearquivo & "_" & Right(processamento, 3) & ".os", nomearquivosaida & ".pdf", local, estadual, nacional, lote, codpostagem, codadministrativo)
                            local = 0
                            estadual = 0
                            nacional = 0
                        End If
                        nomearquivosaida = nomearquivo & "_" & Right(fatura.CIF.TipoRegistro, 3) & "_" & arq.ToString.PadLeft(3, "0")
                        If SomenteNumeros(fatura.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "0" Then
                            local += 1
                        ElseIf SomenteNumeros(fatura.Destinatario.Endereco.CEP).PadLeft(8, "0").Substring(0, 1) = "1" Then
                            estadual += 1
                        Else
                            nacional += 1
                        End If
                        lote = fatura.CIF.CodigoCIF.Substring(10, 5)
                        label.Text = texto & "Gerando os PDF's (" & nomearquivosaida & ") ..." & cont

                        DirectCast(fatura, Layout).ModeloFatura.ConteudoVerso(6) &= Space(2) & "Arq:" & nomearquivosaida

                        If cont2 = 1 Then
                            Dim pdfnovo As String = diretorio & nomearquivosaida
                            Dim fs As New FileStream(pdfnovo & ".PDF", FileMode.Create, FileAccess.Write, FileShare.None)

                            If fatura.CIF.TipoRegistro = "A4" Then
                                documento = New Document(PageSize.A4)
                            ElseIf fatura.CIF.TipoRegistro = "A3C" Then
                                documento = New Document(PageSize.A3.Rotate)
                            ElseIf fatura.CIF.TipoRegistro = "A3N" Then
                                documento = New Document(PageSize.A3.Rotate)
                            Else
                            End If
                            escritor = PdfWriter.GetInstance(documento, fs)
                        End If
                        Application.DoEvents()

                        documento.Open()

                        Dim cb As PdfContentByte = escritor.DirectContent
                        cb.SetColorFill(BaseColor.BLACK)

                        FontFactory.Register("C:\Windows\Fonts\arial.ttf")
                        FontFactory.Register("C:\Windows\Fonts\arialbd.ttf")
                        FontFactory.Register("C:\Windows\Fonts\COUR.TTF")

                        Select Case fatura.Remetente.Apelido
                            Case "Lopes"
                                criarFrenteLOPES(escritor, imagemfrente, cb, fatura)
                                documento.NewPage()
                                criarVersoLOPES(escritor, imagemverso, cb, fatura)
                                documento.NewPage()
                        End Select
                    End If
                Next
                If cont > 0 Then
                    ArquivoOS.Fatura(diretorio, nomearquivo & "_" & Right(processamento, 3) & ".os", nomearquivosaida & ".pdf", local, estadual, nacional, lote, codpostagem, codadministrativo)
                End If
                If Not documento Is Nothing Then
                    documento.Close()
                    escritor.Close()
                End If
            Next
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Shared Sub criarVersoLOPES(ByVal escritor As PdfWriter, ByVal imagem As String, ByVal cb As PdfContentByte, ByVal fatura As Layout)
        Try
            Dim x As Double = 0.0
            Dim y As Double = 0.0
            Dim rotacao As Double = 0

            If imagem.Trim <> "" Then
                If fatura.CIF.TipoRegistro = "A4" Then
                    Dim img As New PdfReader(diretorioPDF & imagem)
                    Dim pagina As PdfImportedPage = escritor.GetImportedPage(img, 1)
                    cb.AddTemplate(pagina, 1, 0, 0, 1, 0, 0)
                    x = 0
                    y = 0
                ElseIf fatura.CIF.TipoRegistro = "A3C" Then
                    Dim img As New PdfReader(diretorioPDF & imagem)
                    img.GetPageN(1).Put(PdfName.ROTATE, New PdfNumber(270))
                    Dim pagina As PdfImportedPage = escritor.GetImportedPage(img, 1)
                    Dim imgrotate As Image = Image.GetInstance(pagina)
                    imgrotate.RotationDegrees = 90.0F
                    imgrotate.SetAbsolutePosition(0, 0)
                    cb.AddImage(imgrotate)
                    x = 210
                    y = 282
                    rotacao = 180
                ElseIf fatura.CIF.TipoRegistro = "A3N" Then
                    Dim img As New PdfReader(diretorioPDF & imagem)
                    img.GetPageN(1).Put(PdfName.ROTATE, New PdfNumber(270))
                    Dim pagina As PdfImportedPage = escritor.GetImportedPage(img, 1)
                    Dim imgrotate As Image = Image.GetInstance(pagina)
                    imgrotate.RotationDegrees = 270.0F
                    imgrotate.SetAbsolutePosition(0, 0)
                    cb.AddImage(imgrotate)
                    x = 200
                    y = 0
                End If
            End If

            Dim ceppostnet As New BarcodePostnet

            ceppostnet.Code = SomenteNumeros(fatura.Destinatario.Endereco.CEP).PadLeft(8, "0")

            Dim datamatrix As New BarcodeDatamatrix
            datamatrix.Generate(DataMatrix2D(fatura))

            Dim codigocif As New Barcode128

            With fatura.ModeloFatura

                If fatura.CIF.TipoRegistro = "A3C" Then
                    inserirImagem(cb, datamatrix.CreateImage(), x - 42, y - 160.5, 0, 100, rotacao)
                    inserirImagem(cb, ceppostnet.CreateImageWithBarcode(cb, BaseColor.BLACK, BaseColor.BLACK), x - 59, y - 145.5, 0, 100, rotacao)
                    escreverFrase(cb, x - 59, y - 151.5, 195, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(0), BaseColor.BLACK)
                    escreverFrase(cb, x - 59, y - 155, 195, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(1), BaseColor.BLACK)
                    escreverFrase(cb, x - 59, y - 158.5, 195, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(2), BaseColor.BLACK)
                    escreverFrase(cb, x - 59, y - 162, 195, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(3), BaseColor.BLACK)
                    escreverFrase(cb, x - 59, y - 165.5, 195, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(4), BaseColor.BLACK)

                    escreverFrase(cb, x - 105, y - 236, x - 195, 1, 5, Element.ALIGN_CENTER, 0, 180, .ConteudoVerso(5), BaseColor.BLACK)
                    codigocif.Code = .ConteudoVerso(5).Replace("Arial|10|0|", "")
                    codigocif.Font = Nothing
                    codigocif.BarHeight = Utilities.MillimetersToPoints(10)
                    inserirImagem(cb, codigocif.CreateImageWithBarcode(cb, BaseColor.BLACK, BaseColor.BLACK), x - 73.5, y - 233, 0, 100, rotacao)

                    escreverFrase(cb, x - 162, y - 270.5, 0 - x, 1, 5, Element.ALIGN_RIGHT, 0, 0, .ConteudoVerso(6), BaseColor.BLACK)

                Else
                    inserirImagem(cb, datamatrix.CreateImage(), 42 + x, 160.5, 0 + x, 100, rotacao)
                    inserirImagem(cb, ceppostnet.CreateImageWithBarcode(cb, BaseColor.BLACK, BaseColor.BLACK), 59 + x, 145.5, 0 + x, 100, rotacao)
                    escreverFrase(cb, 59 + x, 151.5, 195 + x, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(0), BaseColor.BLACK)
                    escreverFrase(cb, 59 + x, 155, 195 + x, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(1), BaseColor.BLACK)
                    escreverFrase(cb, 59 + x, 158.5, 195 + x, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(2), BaseColor.BLACK)
                    escreverFrase(cb, 59 + x, 162, 195 + x, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(3), BaseColor.BLACK)
                    escreverFrase(cb, 59 + x, 165.5, 195 + x, 1, 5, Element.ALIGN_LEFT, 0, rotacao, .ConteudoVerso(4), BaseColor.BLACK)

                    escreverFrase(cb, 15 + x, 227, 195 + x, 1, 5, Element.ALIGN_CENTER, 0, rotacao, .ConteudoVerso(5), BaseColor.BLACK)
                    codigocif.Code = .ConteudoVerso(5).Replace("Arial|10|0|", "")
                    codigocif.Font = Nothing
                    codigocif.BarHeight = Utilities.MillimetersToPoints(10)
                    inserirImagem(cb, codigocif.CreateImageWithBarcode(cb, BaseColor.BLACK, BaseColor.BLACK), 73.5 + x, 224, 0 + x, 100, rotacao)

                    escreverFrase(cb, 192 + x, 270.5, 0 + x, 1, 5, Element.ALIGN_RIGHT, 0, 0, .ConteudoVerso(6), BaseColor.BLACK)

                End If

            End With
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Shared Sub criarFrenteLOPES(ByVal escritor As PdfWriter, ByVal imagem As String, ByVal cb As PdfContentByte, ByVal fatura As Layout)
        Try
            Dim begin As Double = 0.0
            Dim posicao As Double = 0.0

            If imagem.Trim <> "" Then
                If fatura.CIF.TipoRegistro = "A4" Then
                    Dim img As New PdfReader(diretorioPDF & imagem)
                    Dim pagina As PdfImportedPage = escritor.GetImportedPage(img, 1)
                    cb.AddTemplate(pagina, 1, 0, 0, 1, 0, 0)
                ElseIf fatura.CIF.TipoRegistro = "A3C" Then
                    Dim img As New PdfReader(diretorioPDF & imagem)
                    img.GetPageN(1).Put(PdfName.ROTATE, New PdfNumber(270))
                    Dim pagina As PdfImportedPage = escritor.GetImportedPage(img, 1)
                    Dim imgrotate As Image = Image.GetInstance(pagina)
                    imgrotate.RotationDegrees = 270.0F
                    imgrotate.SetAbsolutePosition(0, 0)
                    cb.AddImage(imgrotate)
                    posicao = 46.5
                ElseIf fatura.CIF.TipoRegistro = "A3N" Then
                    Dim img As New PdfReader(diretorioPDF & imagem)
                    img.GetPageN(1).Put(PdfName.ROTATE, New PdfNumber(270))
                    Dim pagina As PdfImportedPage = escritor.GetImportedPage(img, 1)
                    Dim imgrotate As Image = Image.GetInstance(pagina)
                    imgrotate.RotationDegrees = 270.0F
                    imgrotate.SetAbsolutePosition(0, 0)
                    cb.AddImage(imgrotate)
                    begin = 210.0
                    posicao = 46.5
                End If
            End If


                Dim code2de5 As New BarcodeInter25
                Dim y As Integer = 49


            criarBox(cb, begin + 32, 42.5, 140, 6.5, BaseColor.BLACK, BaseColor.BLACK)


            With fatura.ModeloFatura

                If fatura.CIF.TipoRegistro = "A3C" Then
                    escreverFrase(cb, begin + 18.18, 13.11, begin + 195, 1, 4, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(0), BaseColor.BLACK)
                    escreverFrase(cb, begin - 120.0, 22.0, begin + 195, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(1), BaseColor.BLACK)
                    escreverFrase(cb, begin + 61.5, 13.11, begin + 195, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(2), BaseColor.BLACK)
                    escreverFrase(cb, begin + 125.5, 13.11, begin + 175.5, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(3), BaseColor.BLACK)
                    escreverFrase(cb, begin + 150.5, 13.11, begin + 200.5, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(4), BaseColor.BLACK)
                    escreverFrase(cb, begin + 73, 22.0, begin + 99, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(5), BaseColor.BLACK)
                    escreverFrase(cb, begin + 78, 32.0, begin + 104, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(6), BaseColor.BLACK)
                    escreverFrase(cb, begin + 11, 32.0, begin + 51, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(7), BaseColor.BLACK)
                    escreverFrase(cb, begin + 43, 32.0, begin + 83, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(8), BaseColor.BLACK)
                    escreverFrase(cb, begin + 76, 40.0, begin + 118, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(9), BaseColor.BLACK)
                    escreverFrase(cb, begin + 25, 45.0, begin + 179, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(10), BaseColor.WHITE)
                    escreverFrase(cb, begin + 40, 48.0, begin + 156, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(11), BaseColor.WHITE)
                    escreverFrase(cb, begin + 360, 288.0, begin + 420, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(12), BaseColor.BLACK)
                    escreverFrase(cb, begin + 16, 175.0, begin + 103, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(13), BaseColor.BLACK)
                    escreverFrase(cb, begin + 43, 102.0, begin + 140, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(14), BaseColor.BLACK)
                    escreverFrase(cb, begin + 82, 102.0, begin + 179, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(15), BaseColor.BLACK)
                    escreverFrase(cb, begin + 155, 102.0, begin + 245, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(16), BaseColor.BLACK)
                    escreverFrase(cb, begin + 107, 175.0, begin + 187, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(17), BaseColor.BLACK)
                Else
                    escreverFrase(cb, begin + 18.18, 11.11, begin + 195, 1, 4, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(0), BaseColor.BLACK)
                    escreverFrase(cb, begin + 127, 11.11, begin + 158, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(1), BaseColor.BLACK)
                    escreverFrase(cb, begin + 173, 11.11, begin + 189, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(2), BaseColor.BLACK)
                    escreverFrase(cb, begin + 10, 22.0, begin + 42, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(3), BaseColor.BLACK)
                    escreverFrase(cb, begin + 43, 22.0, begin + 74, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(4), BaseColor.BLACK)
                    escreverFrase(cb, begin + 76, 22.0, begin + 108, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(5), BaseColor.BLACK)
                    escreverFrase(cb, begin + 10, 32.0, begin + 42, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(6), BaseColor.BLACK)
                    escreverFrase(cb, begin + 43, 32.0, begin + 84, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(7), BaseColor.BLACK)
                    escreverFrase(cb, begin + 76, 32.0, begin + 118, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(8), BaseColor.BLACK)
                    escreverFrase(cb, begin + 76, 40.0, begin + 118, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(9), BaseColor.BLACK)
                    escreverFrase(cb, begin + 25, 45.0, begin + 179, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(10), BaseColor.WHITE)
                    escreverFrase(cb, begin + 40, 48.0, begin + 156, 1, 4, Element.ALIGN_CENTER, 0, 0, .ConteudoFrente(11), BaseColor.WHITE)
                    escreverFrase(cb, begin + 140, 180.0, begin + 200, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(12), BaseColor.BLACK)
                    escreverFrase(cb, begin + 38, 186.0, begin + 135, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(13), BaseColor.BLACK)
                    escreverFrase(cb, begin + 43, 102.0, begin + 140, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(14), BaseColor.BLACK)
                    escreverFrase(cb, begin + 82, 102.0, begin + 179, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(15), BaseColor.BLACK)
                    escreverFrase(cb, begin + 155, 102.0, begin + 245, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(16), BaseColor.BLACK)
                    escreverFrase(cb, begin + 107, 175.0, begin + 187, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(17), BaseColor.BLACK)
                End If


                'criarFichaCompensacaoFatura(cb, 0, 0)

                escreverFrase(cb, begin + 70, 207.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(19), BaseColor.BLACK)
                escreverFrase(cb, begin + 16, 213.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(20), BaseColor.BLACK)
                escreverFrase(cb, begin + 163, 213.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(21), BaseColor.BLACK)
                escreverFrase(cb, begin + 27, 216.5, begin + 118, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(22), BaseColor.BLACK)
                escreverFrase(cb, begin + 161.5, 219.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(23), BaseColor.BLACK)
                escreverFrase(cb, begin + 20, 225.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(24), BaseColor.BLACK)
                escreverFrase(cb, begin + 115, 225.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(24), BaseColor.BLACK)
                escreverFrase(cb, begin + 49, 225.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(25), BaseColor.BLACK)
                escreverFrase(cb, begin + 158, 225.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(26), BaseColor.BLACK)
                escreverFrase(cb, begin + 81, 225.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(27), BaseColor.BLACK)
                escreverFrase(cb, begin + 96, 225.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(28), BaseColor.BLACK)
                'escreverFrase(cb, begin +95, 224, begin +115, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(29), BaseColor.BLACK)
                escreverFrase(cb, begin + 66, 231.5, begin + 125, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(30), BaseColor.BLACK)
                escreverFrase(cb, begin + 181, 231.5, begin + 15, 1, 3, Element.ALIGN_RIGHT, 0, 0, .ConteudoFrente(31), BaseColor.BLACK)

                y = 240
                For i As Integer = 32 To 38
                    escreverFrase(cb, begin + 15, y, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(i), BaseColor.BLACK)
                    y += 3
                Next

                escreverFrase(cb, begin + 26, 266.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(39), BaseColor.BLACK)
                escreverFrase(cb, begin + 26, 269.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(40), BaseColor.BLACK)
                escreverFrase(cb, begin + 26, 272.5, begin + 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(41), BaseColor.BLACK)

                code2de5.Code = .ConteudoFrente(42)
                code2de5.BarHeight = 36
                code2de5.Font = Nothing
                code2de5.N = 2.5
                cb.AddTemplate(code2de5.CreateTemplateWithBarcode(cb, BaseColor.BLACK, BaseColor.BLACK), Utilities.MillimetersToPoints(begin + 25), Utilities.MillimetersToPoints(297 - 291))

                y = 67
                For i As Integer = 43 To .ConteudoFrente.Count - 1
                    If fatura.CIF.TipoRegistro = "A4" Then
                        escreverFrase(cb, 15.5, y, 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(i), BaseColor.BLACK)
                    ElseIf fatura.CIF.TipoRegistro = "A3C" Then
                        escreverFrase(cb, begin + 238, y - posicao, begin + 418, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(i), BaseColor.BLACK)
                    ElseIf fatura.CIF.TipoRegistro = "A3N" Then
                        escreverFrase(cb, 9.0, y - posicao, 195, 1, 3, Element.ALIGN_LEFT, 0, 0, .ConteudoFrente(i), BaseColor.BLACK)
                    End If
                    y += 2.5
                Next

            End With
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

End Class

