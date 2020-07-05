Imports Newtonsoft.Json
Class MainWindow
    Dim contentList As List(Of Object) = New List(Of Object)
    Public Function UUID() As String
        Const dic As String = "0123456789abcdef"
        Dim tmp As String = ""
        For i = 0 To 7
            tmp += dic.Substring(Int(Rnd() * 16), 1)
        Next i
        tmp += "-"
        For i = 0 To 3
            tmp += dic.Substring(Int(Rnd() * 16), 1)
        Next i
        tmp += "-4"
        For i = 0 To 2
            tmp += dic.Substring(Int(Rnd() * 16), 1)
        Next i
        tmp += "-"
        For i = 0 To 3
            tmp += dic.Substring(Int(Rnd() * 16), 1)
        Next i
        tmp += "-"
        For i = 0 To 11
            tmp += dic.Substring(Int(Rnd() * 16), 1)
        Next i
        UUID = tmp
    End Function
    Private Sub 点击() 'Handles [Try].Click
        Dim a
        a = Newtonsoft.Json.JsonConvert.DeserializeObject("{}")
        a("serialize_name") = "防刷皮肤包"
        a("localization_name") = "防刷皮肤包"
        Dim json As String = Newtonsoft.Json.JsonConvert.SerializeObject(contentList, Formatting.None)
        a("skins") = JsonConvert.DeserializeObject(json)
        TB1.Text = JsonConvert.SerializeObject(a).Replace("\""", """")
    End Sub
    Private Sub 移动窗口() Handles Bkg.MouseLeftButtonDown
        Me.DragMove()
    End Sub
    Private Sub 下一步1() Handles Next1.Click
        Start_Title.Opacity = 0.5
        Minifest_title.Opacity = 1
        Start.Visibility = Visibility.Hidden
        Minifest.Visibility = Visibility.Visible
        Randomize()
        uuid1.Text = UUID()
        uuid2.Text = UUID()
        If IO.Directory.Exists(System.Environment.CurrentDirectory + "\spm_tmp") Then MsgBox("检测到临时文件夹已经存在，请自行确认后手动删除！")
    End Sub
    Private Sub UUID重算() Handles UUIDreset.Click
        uuid1.Text = UUID()
        uuid2.Text = UUID()
    End Sub
    Private Sub 确定minefest() Handles Next2.Click
        Dim minifest_index As String = My.Settings.minefest_demo
        minifest_index = minifest_index.Replace("%ver1", Int(ver1.Text).ToString + "," + Int(ver2.Text).ToString + "," + Int(ver3.Text).ToString)
        minifest_index = minifest_index.Replace("%ver2", Int(ver1.Text).ToString + "," + Int(ver2.Text).ToString + "," + Int(ver3.Text).ToString)
        minifest_index = minifest_index.Replace("%uuid1", uuid1.Text)
        minifest_index = minifest_index.Replace("%uuid2", uuid2.Text)
        minifest_index = minifest_index.Replace("%name", pack_name.Text)
        minifest_index = minifest_index.Replace("%des", des.Text)
        Try
            IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "\spm_tmp")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        IO.File.WriteAllText(System.Environment.CurrentDirectory + "\spm_tmp\manifest.json", minifest_index)
        Minifest_title.Opacity = 0.5
        Add_title.Opacity = 1
        Minifest.Visibility = Visibility.Hidden
        Add.Visibility = Visibility.Visible
    End Sub
    Private Sub 选择贴图() Handles png_select.Click
        Dim ld As New Forms.OpenFileDialog
        ld.CheckPathExists = True
        ld.Filter = "PNG文件 (*.png)|*.png"
        ld.Title = "打开皮肤文件"
        ld.ShowDialog()
        If IO.File.Exists(ld.FileName) = True Then
            file.Text = ld.FileName
        End If
        Dim uriSource As Uri
        Try
            uriSource = New Uri(ld.FileName)
            Dim decoder As New PngBitmapDecoder(uriSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default)
            Dim bitmapSource As BitmapSource = decoder.Frames(0)
            preview.Source = bitmapSource
        Catch ex As Exception
            uriSource = New Uri("pack://application:,,,/SkinPackMaker;component/none.jpg")
            Dim decoder As New JpegBitmapDecoder(uriSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default)
            Dim bitmapSource As BitmapSource = decoder.Frames(0)
            preview.Source = bitmapSource
            file.Text = ex.Message
        End Try
    End Sub
    Private Sub 添加皮肤() Handles skin_add.Click
        Dim OK = True
        If IO.File.Exists((System.Environment.CurrentDirectory + "\spm_tmp\" + skin_name.Text + ".png")) = True Then
            MsgBox("此皮肤名已被添加")
            OK = False
        End If
        If IO.File.Exists(file.Text) = False Then
            MsgBox("贴图文件错误")
            OK = False
            'IO.File.Copy(file.Text, (System.Environment.CurrentDirectory + "\spm_tmp\" + skin_name.Text + ".png"))
        End If
        If skin_name.Text = "" Then
            MsgBox("皮肤名为空")
            OK = False
        End If
        If Slim.IsChecked = False And Classic.IsChecked = False Then
            MsgBox("尚未选择模型")
            OK = False
        End If
        If OK Then
            IO.File.Copy(file.Text, (System.Environment.CurrentDirectory + "\spm_tmp\" + skin_name.Text + ".png"))
            Dim s
            s = Newtonsoft.Json.JsonConvert.DeserializeObject("{}")
            s("texture") = skin_name.Text + ".png"
            s("localization_name") = skin_name.Text
            s("type") = "free"
            If Classic.IsChecked = True Then
                s("geometry") = "geometry.humanoid.custom"
            Else
                s("geometry") = "geometry.humanoid.customSlim"
            End If
            contentList.Add(s)
            count.Text = contentList.LongCount
            Dim uriSource = New Uri("pack://application:,,,/SkinPackMaker;component/none.jpg")
            Dim decoder As New JpegBitmapDecoder(uriSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default)
            Dim bitmapSource As BitmapSource = decoder.Frames(0)
            preview.Source = bitmapSource
            skin_name.Text = ""
            file.Text = ""
        End If
    End Sub
    Private Sub 开始生成() Handles Next3.Click
        If contentList.LongCount = 0 Then
            MsgBox("皮肤包里啥都没有")
        Else
            Dim a
            a = Newtonsoft.Json.JsonConvert.DeserializeObject("{}")
            a("serialize_name") = pack_name.Text
            a("localization_name") = pack_name.Text
            Dim json As String = Newtonsoft.Json.JsonConvert.SerializeObject(contentList, Formatting.None)
            a("skins") = JsonConvert.DeserializeObject(json)
            IO.File.WriteAllText(System.Environment.CurrentDirectory + "\spm_tmp\skins.json", JsonConvert.SerializeObject(a).Replace("\""", """"))
            count_final.Text = contentList.LongCount
            Make_title.Opacity = 1
            Add_title.Opacity = 0.5
            Add.Visibility = Visibility.Hidden
            Make.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub 选择生成目录() Handles mcpack_select.Click
        Dim ld As New Forms.SaveFileDialog
        ld.Filter = "Minecraft Package文件 (*.mcpack)|*.mcpack"
        ld.Title = "保存皮肤文件"
        ld.ShowDialog()
        If ld.FileName <> "" Then
            file1.Text = ld.FileName
        End If
    End Sub
    Private Sub 生成mcpack() Handles Next4.Click
        If file1.Text <> "尚未选择位置" Then
            status.Text = "编写lang文件"
            IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "\spm_tmp\texts")
            Dim t As System.IO.StreamWriter = New System.IO.StreamWriter(System.Environment.CurrentDirectory + "\spm_tmp\texts\zh_CN.lang", True, System.Text.Encoding.UTF8)
            t.WriteLine("skinpack." + pack_name.Text + "=" + pack_name.Text)
            For Each a As Object In contentList
                t.WriteLine("skin." + pack_name.Text + "." + a("localization_name") + "=" + a("localization_name"))
            Next
            t.Close()
            status.Text = "生成Minecraft Package"
            Try
                IO.Compression.ZipFile.CreateFromDirectory(System.Environment.CurrentDirectory + "\spm_tmp", file1.Text)
                status.Text = "生成完毕！"
            Catch ex As Exception
                status.Text = ex.Message
            Finally
                IO.Directory.Delete(System.Environment.CurrentDirectory + "\spm_tmp", True)
            End Try
            Next4.Visibility = Visibility.Hidden
            Next5.Visibility = Visibility.Visible
        Else
            MsgBox("尚未选择保存位置")
        End If
    End Sub
    Private Sub 关闭() Handles Kill.Click
        If IO.Directory.Exists(System.Environment.CurrentDirectory + "\spm_tmp") Then IO.Directory.Delete(System.Environment.CurrentDirectory + "\spm_tmp", True)
        Me.Close()
    End Sub
    Private Sub 重制() Handles Next5.Click
        Dim sdash As New MainWindow
        sdash.Show()
        Me.Close()
    End Sub
End Class
