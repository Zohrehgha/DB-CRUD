Public Class Form1
    'connect to database access
    Dim conn As New ADODB.Connection
    Dim rs As New ADODB.Recordset    'recordset for security table
    ' connect to sql database
    Dim cn As New OleDb.OleDbConnection
    Dim da As OleDb.OleDbDataAdapter
    Dim ds As New DataSet
    Dim Maxrecord As Integer
    Dim CurrentRow As Integer
    'create an array of datarows 
    ' so the result of select method can live in this array
    Dim FoundRows() As Data.DataRow
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'setting up the ADO objects(access)
        conn.Provider = "Microsoft.Jet.OleDB.4.0"
        ' Setting up the jet DB driver (access) 
        conn.ConnectionString = "C:\ITD\Term 3\Visual Basic.Net\assignment\Final\EmployeeDB.mdb"
        conn.Open()
        rs.Open("select * from securityTable", conn, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockPessimistic)
        'hiding tab control
        TabControl1.TabPages.Remove(TabControl1.TabPages("tabpage2"))

        'TODO: This line of code loads data into the sql table.
        cn.ConnectionString = "Provider = SQLNCLI11.0;Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\ITD\Term 3\Visual Basic.Net\assignment\Final\Employee\Employee\EmployeeDatabase.mdf;Integrated Security = SSPI;database=thingy"
        cn.Open()
        'for sql table
        Dim SqlStr As String
        SqlStr = "Select * from EmployeeTable"
        da = New OleDb.OleDbDataAdapter(SqlStr, cn)
        da.Fill(ds, "EmployeeTable")
        Maxrecord = ds.Tables("EmployeeTable").Rows.Count - 1
        'connect dataGridView to EmployeeTable
        DataGridView1.DataSource = ds.Tables(0)

    End Sub
    'verify clientID and Password by reading the info from the database 
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Or TextBox2.Text = "" Then
            MsgBox("Plz Fill Client ID and Password")
        Else
            Dim Criteria As String
            Criteria = "userName = '" + TextBox1.Text + "'"
            rs.MoveFirst()
            rs.Find(Criteria)
            If rs.EOF Then
                MsgBox("This username does not exit")
            Else
                If (rs.Fields("userName").Value.ToString = TextBox1.Text And rs.Fields("Password").Value.ToString = TextBox2.Text) Then
                    MessageBox.Show("Login succesfully")
                    'access to tab control to user
                    TabControl1.TabPages.Add(TabPage2)
                    TabControl1.SelectedTab = TabControl1.TabPages("Employee")
                    'TabControl1.TabPages.Remove(TabPage1)
                    TextBox1.Clear()
                    TextBox2.Clear()
                Else
                    MessageBox.Show("Invalid Account")
                    TextBox1.Clear()
                    TextBox2.Clear()
                End If
            End If
        End If
    End Sub
    'adding new employee to sql database
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim Criteria As String
        If TextBox3.Text = "" Then
            MessageBox.Show("Please Enter an Employee ID to Add")
            Exit Sub
        End If
        If TextBox4.Text = "" Or TextBox5.Text = "" Or TextBox6.Text = "" Then
            MessageBox.Show("Incomplete Data, please fill all fields")
            Exit Sub
        End If
        Criteria = "EmployeeID = '" + TextBox3.Text + "'"
        FoundRows = ds.Tables("EmployeeTable").Select(Criteria)
        If FoundRows.Length = 0 Then
            Dim NewRow As DataRow = ds.Tables("EmployeeTable").NewRow()
            'the next line is needed so VB Can execute the SQL statement 
            Dim Cb As New OleDb.OleDbCommandBuilder(da)
            NewRow.Item("EmployeeID") = Convert.ToInt32(TextBox3.Text)
            NewRow.Item("Name") = TextBox4.Text
            NewRow.Item("SurName") = TextBox5.Text
            NewRow.Item("Age") = TextBox6.Text
            NewRow.Item("HiringDate") = Convert.ToDateTime(DateTimePicker1.Text)

            ds.Tables.Item("EmployeeTable").Rows.Add(NewRow)
            da.Update(ds, "EmployeeTable")
            Maxrecord = Maxrecord + 1
            MessageBox.Show("Record Added succesfully!!!")
            Call ClearForm()
        Else
            MessageBox.Show("Duplicate Record!!!, Try another Employee ID")
        End If
    End Sub
    Public Sub ClearForm()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        TextBox6.Clear()
    End Sub
    'modifying data into sql table
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim Strtofind As String
        If TextBox3.Text = "" Then
            MessageBox.Show("Please Enter a Valid Employee ID to find")
            Exit Sub
        End If
        Strtofind = "EmployeeID = '" + TextBox3.Text + "'"
        FoundRows = ds.Tables("EmployeeTable").Select(Strtofind)
        Dim RowIndex As Integer
        If FoundRows.Length = 0 Then
            MessageBox.Show("Record Not Found, try again")
        Else
            'save the data 
            Dim Cb As New OleDb.OleDbCommandBuilder(da)
            RowIndex = ds.Tables("EmployeeTable").Rows.IndexOf(FoundRows(0))
            ds.Tables("EmployeeTable").Rows(RowIndex).Item(0) = TextBox3.Text
            ds.Tables("EmployeeTable").Rows(RowIndex).Item(1) = TextBox4.Text
            ds.Tables("EmployeeTable").Rows(RowIndex).Item(2) = TextBox5.Text
            ds.Tables("EmployeeTable").Rows(RowIndex).Item(3) = TextBox6.Text
            ds.Tables("EmployeeTable").Rows(RowIndex).Item(4) = DateTimePicker1.Text
            da.Update(ds, "EmployeeTable")
            MessageBox.Show("Record Modified Succesfully")
        End If
    End Sub
    'search by id
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim Criteria As String
        If TextBox3.Text = "" Then
            MessageBox.Show("Please Enter a Valid Employee ID to find")
            Exit Sub ' = return
        End If
        Criteria = "EmployeeID = '" + TextBox3.Text + "'"
        FoundRows = ds.Tables("EmployeeTable").Select(Criteria)
        If FoundRows.Length = 0 Then 'if the array is empty
            'which it means record not found 
            MessageBox.Show("Record not found !!!")
            Exit Sub
        Else
            'record has been found 
            Dim RowIndex As Integer
            RowIndex = ds.Tables("EmployeeTable").Rows.IndexOf(FoundRows(0))
            Call ShowRecord(RowIndex)
            CurrentRow = ds.Tables("EmployeeTable").Rows.IndexOf(FoundRows(0))
        End If
    End Sub
    'Delete data from sql database
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim Strtofind As String
        If TextBox3.Text = "" Then
            MessageBox.Show("Please Enter a Valid Employee ID to delete")
            Exit Sub
        End If
        Strtofind = "EmployeeID = '" + TextBox3.Text + "'"
        FoundRows = ds.Tables("EmployeeTable").Select(Strtofind)
        Dim RowIndex As Integer
        If FoundRows.Length = 0 Then
            MessageBox.Show("Record Not Found, try another ISBN")
        Else
            'Delete the data 
            Dim Cb As New OleDb.OleDbCommandBuilder(da)
            Dim result As Integer
            RowIndex = ds.Tables("EmployeeTable").Rows.IndexOf(FoundRows(0))
            Call ShowRecord(RowIndex)
            result = MessageBox.Show("Are you Sure?", "Deleting Record", MessageBoxButtons.YesNo)
            If result = vbYes Then
                ds.Tables("EmployeeTable").Rows(RowIndex).Delete()
                Call ClearForm()
                MessageBox.Show("Record deleted Succesfully!!!")
                Maxrecord = Maxrecord - 1
            End If
        End If
    End Sub
    'showing data from sql table into form
    Public Sub ShowRecord(ByVal ThisRow As Integer)
        TextBox3.Text = ds.Tables("EmployeeTable").Rows(ThisRow).Item(0)
        TextBox4.Text = ds.Tables("EmployeeTable").Rows(ThisRow).Item(1)
        TextBox5.Text = ds.Tables("EmployeeTable").Rows(ThisRow).Item(2)
        TextBox6.Text = ds.Tables("EmployeeTable").Rows(ThisRow).Item(3)
        DateTimePicker1.Text = ds.Tables("EmployeeTable").Rows(ThisRow).Item(4)
    End Sub
End Class
