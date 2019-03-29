Imports System.IO

Public Class frmPicrossGame
    ' Dimension of each cell in the grid
    Const CellWidth As Integer = 25
    Const CellHeight As Integer = 25
    ' Offset from the top-left corner of the window
    Const xOffset As Integer = 100
    Const yOffset As Integer = 90
    ' Color for empty cell
    Private Default_Backcolor As Color = Color.WhiteSmoke
    ' Color for original puzzle values
    Private Fixed_Forecolor As Color = Color.Purple
    Private Fixed_Backcolor As Color = Color.SkyBlue
    ' Color for user-inserted values
    Private User_Forecolor As Color = Color.Purple
    Private User_Backcolor As Color = Color.WhiteSmoke
    ' The number currently selected for insertion
    Private SelectedNumber As Integer
    ' Stacks to keep track of all the moves
    Private Moves As Stack(Of String)
    Private RedoMoves As Stack(Of String)
    ' Keep track of filename to save to
    Private SaveFileName As String = String.Empty
    ' Used to represent the values in the grid
    Private Actual(9, 9) As Integer
    ' Has the game started?
    Private GameStarted As Boolean = False
    Private possible(9, 9) As String
    Private DefaultRow As Integer = 10
    Private DefaultColumn As Integer = 10
    Private text_xOffset As Integer = 30
    Private text_yOffset As Integer = 30
    '==============================================================='
    ' Draw Label controls to represent each cell in the grids
    '==============================================================='
    Dim listOfLabels As New List(Of String)
    Public Sub _ClearBoard()
        For Each item As String In listOfLabels
            Dim lbl As New Label
            With lbl.Name = item
                Me.Controls.RemoveByKey(item)
            End With
        Next
    End Sub
    Public Sub DrawBoard(ByVal _row As Integer, ByVal _col As Integer)
        ' Initialize locations for cells and texts
        Dim cellLocation As New Point
        Dim row_textLocation As New Point
        Dim col_textLocation As New Point
        For row As Integer = 1 To _row
            ' Draw horizontal textbox
            row_textLocation.Y = row * (CellHeight + 1) + yOffset
            row_textLocation.X = text_xOffset + 35
            Dim lblRow As New Label
            With lblRow
                .Name = "rowtb" + row.ToString()
                .Location = row_textLocation
                .Width = CellWidth + 35
                .Height = CellHeight
                .BackColor = Color.Wheat
                .Font = New Font(.Font, .Font.Style Or FontStyle.Bold)
            End With
            Me.Controls.Add(lblRow)
            listOfLabels.Add(lblRow.Name)
            For col As Integer = 1 To _col
                ' Draw vertical textbox
                If (row = 1) Then
                    Dim lblColumn As New Label
                    col_textLocation.Y = 55
                    col_textLocation.X = col * (CellWidth + 1) + xOffset
                    With lblColumn
                        .Name = "coltb" + col.ToString()
                        .Location = col_textLocation
                        .Width = CellWidth - 1
                        .Height = 60
                        .BackColor = Color.Wheat
                        .Font = New Font(.Font, .Font.Style Or FontStyle.Bold)
                        'AddHandler column_textbox.Paint, AddressOf RotateTheLabel
                    End With
                    Me.Controls.Add(lblColumn)
                    listOfLabels.Add(lblColumn.Name)
                End If
                ' Draw cell 
                cellLocation.X = col * (CellWidth + 1) + xOffset
                cellLocation.Y = row * (CellHeight + 1) + yOffset
                Dim lbl As New Label
                With lbl
                    .Name = col.ToString() & row.ToString()
                    .BorderStyle = BorderStyle.FixedSingle
                    .Location = cellLocation
                    .Width = CellWidth
                    .Height = CellHeight
                    .TextAlign = ContentAlignment.MiddleCenter
                    .BackColor = Default_Backcolor
                    .Font = New Font(.Font, .Font.Style Or FontStyle.Bold)
                    AddHandler lbl.Click, AddressOf Cell_Click
                End With
                listOfLabels.Add(lbl.Name)
                Me.Controls.Add(lbl)
            Next
        Next
    End Sub
    Private Sub frmPicrossGame_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Initialize the status bar
        tstStatusLabel.Text = String.Empty
        ' Draw the board
        DrawBoard(DefaultRow, DefaultColumn)
        'Dim filename As String = "C:\Users\Administrator\Desktop\Sudoku\Sudoku\MakeSudoku\Models\Readme - Copy.md"
        Dim filename As String = "D:\Picross-projects\Picross Game\Data for Picross\brpicross_data.dat"
        Dim filename_2 As String = "D:\Picross-projects\Picross Game\Data for Picross\bcpicross_data.dat"
        LoadDataFromFile(filename, "rowtb")
        LoadDataFromFile(filename_2, "coltb")
    End Sub
    Private Sub LoadDataFromFile(ByVal FILE_NAME As String, ByVal tbName As String)
        Dim matrix As New List(Of ArrayList)
        Dim TextLine As String = ""
        If System.IO.File.Exists(FILE_NAME) = True Then
            Dim objReader As New System.IO.StreamReader(FILE_NAME)
            Do While objReader.Peek() <> -1
                TextLine = objReader.ReadLine()
                Dim row As New ArrayList
                For Each j In TextLine.Split(" ")
                    'row.Add(Integer.Parse(j))
                    row.Add(j)
                Next
                matrix.Add(row)
            Loop
            For Each i In matrix
                Dim tb As Label = CType(Me.Controls(tbName + i.Item(0).ToString), Label)
                tb.Text = tb.Text + i.Item(2).ToString + " "
                'tb.AppendText(i.Item(2).ToString + " ")
            Next
        Else
            'MessageBox.Show("File Does Not Exist")
        End If
    End Sub
    Public Sub ClearBoard()
        ' Initialize the cells in the board
        For row As Integer = 1 To 9
            For col As Integer = 1 To 9
                SetCell(col, row, 0, 1)
            Next
        Next
    End Sub
    Public Sub SetCell(ByVal col As Integer, ByVal row As Integer, ByVal value As Integer, ByVal erasable As Short)
        ' Locate the particular Label control
        Dim lbl() As Control = Me.Controls.Find(col.ToString() & row.ToString(), True)
        Dim cellLabel As Label = CType(lbl(0), Label)
        ' Save the value in the array 
        Actual(col, row) = value
        ' If erasing a cell, you need to reset the possible values for all cells
        If value = 0 Then
            For r As Integer = 1 To 9
                For c As Integer = 1 To 9
                    If Actual(c, r) = 0 Then possible(c, r) = String.Empty
                Next
            Next
        Else
            possible(col, row) = value.ToString()
        End If
        ' Set the appearance for the Label control
        If value = 0 Then
            ' erasing the cell
            cellLabel.Text = String.Empty
            cellLabel.Tag = erasable
            cellLabel.BackColor = Default_Backcolor
        Else
            If erasable = 0 Then
                ' default puzzle values
                cellLabel.BackColor = Fixed_Backcolor
                cellLabel.ForeColor = Fixed_Forecolor
            Else
                ' user-set value
                cellLabel.BackColor = User_Backcolor
                cellLabel.ForeColor = User_Forecolor
            End If
            cellLabel.Text = value
            cellLabel.Tag = erasable
        End If
    End Sub
    ' Click event for the Label (cell) controls
    Private Sub Cell_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim cellLabel As Label = CType(sender, Label)
        ' If cell is not erasable then exit
        If cellLabel.Tag.ToString() = "0" Then
            Return
        End If
        ' Determine the column and row of the selected cell
        Dim col As Integer = cellLabel.Name.Substring(0, 1)
        Dim row As Integer = cellLabel.Name.ToString().Substring(1, 1)
        ' If erasing a cell
        If SelectedNumber = 0 Then
            ' If cell is empty then no need to erase
            If Actual(col, row) = 0 Then Return
            ' Save the value in the array
            SetCell(col, row, SelectedNumber, 1)
        ElseIf cellLabel.Text = String.Empty Then
            ' else set a value; check if move is valid
            If Not IsMoveValid(col, row, SelectedNumber) Then                           '===(6)==='
                Return
            End If
            ' Save the value in the array
            SetCell(col, row, SelectedNumber, 1)
            ' Saves the move into the stack
            Moves.Push(cellLabel.Name.ToString() & SelectedNumber)
        End If
    End Sub
    Public Function IsMoveValid(ByVal col As Integer, ByVal row As Integer, ByVal value As Integer) As Boolean
        Dim puzzleSolved As Boolean = True
        ' Scan through column
        For r As Integer = 1 To 9
            If Actual(col, r) = value Then      ' duplicate
                Return False
            End If
        Next
        ' Scan through row
        For c As Integer = 1 To 9
            If Actual(c, row) = value Then      ' duplicate
                Return False
            End If
        Next
        ' Scan through minigrid
        Dim startC, startR As Integer
        startC = col - ((col - 1) Mod 3)
        startR = row - ((row - 1) Mod 3)
        For rr As Integer = 0 To 2
            For cc As Integer = 0 To 2
                If Actual(startC + cc, startR + rr) = value Then
                    ' duplicate
                    Return False
                End If
            Next
        Next
        Return True
    End Function
    Private Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        Dim row As Integer
        Dim column As Integer
        If (Not Integer.TryParse(txtRow.Text, row) Or Not Integer.TryParse(txtColumn.Text, column)) Then
            MessageBox.Show("Invalid column and row values, try again!")
        Else
            _ClearBoard()
            DrawBoard(row, column)

        End If
    End Sub
    '=========================================================================================================='
    '=========================================================================================================='

    'Public Sub SolvePuzzle()
    '    ' Write ZIMPL code for BLP formulation
    '    WriteZimplCodeBLPformulation()
    '    ' Run ZIMPL to get .lp file as input of SCIP
    '    RunZIMPL()
    '    ' Run SCIP to solve the puzzle
    '    RunSCIP()
    '    '' Export SCIP result file to the format of the game
    '    'ExportScipResultToArrayAndFileSDO()
    '    '' Show the result on interface
    '    'ShowOnInterface()

    '    'If IsPuzzleSolved() Then
    '    '    Timer.Enabled = False
    '    '    Beep()
    '    '    tstStatusLabel.Text = "*****Puzzle Solved*****"
    '    '    MsgBox("Puzzle solved")
    '    'End If
    'End Sub

    Private Sub cmdSolvePuzzle_Click(sender As Object, e As EventArgs) Handles cmdSolvePuzzle.Click
        FromInterfaceInputToZplInput()
        WriteZimplModelFile()
        RunZIMPL()
        RunSCIP()
        PresentSolution()
    End Sub

    Private Sub FromInterfaceInputToZplInput()
        ' I. READ FILE SAVING DATA OF CURRENT PUZZLE ON INTERFACE and SAVE THE DATA INTO MATRICES
        Dim fileToRead As String
        Dim objReader As StreamReader
        Dim stringLine As String
        Dim extractInfo As String
        Dim errInfo As String = ""

        ' Read the part concerning the size of puzzles and size of clues
        Dim maxNumberOfCluesOnColumns As Integer
        Dim maxNumberOfCluesOnRows As Integer
        Dim numberOfRowsOfPuzzle As Integer
        Dim numberOfColumnsOfPuzzle As Integer
        Try
            fileToRead = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileCurrentInterfaceInput
            ' Get the value for number of rows of the puzzle
            objReader = New StreamReader(fileToRead)
            Do
                stringLine = objReader.ReadLine()
            Loop Until stringLine.Contains("# Puzzle size: number of rows")
            stringLine = objReader.ReadLine()
            extractInfo = Replace(stringLine, " ", "")
            Integer.TryParse(extractInfo, numberOfRowsOfPuzzle)
            ' Get the value for number of columns of the puzzle
            Do
                stringLine = objReader.ReadLine()
            Loop Until stringLine.Contains("# Puzzle size: number of columns")
            stringLine = objReader.ReadLine()
            extractInfo = Replace(stringLine, " ", "")
            Integer.TryParse(extractInfo, numberOfColumnsOfPuzzle)
            ' Get the value for maximum number of possitive clues for rows
            Do
                stringLine = objReader.ReadLine()
            Loop Until stringLine.Contains("# Maximum number of (possitive) clues for rows")
            stringLine = objReader.ReadLine()
            extractInfo = Replace(stringLine, " ", "")
            Integer.TryParse(extractInfo, maxNumberOfCluesOnRows)
            ' Get the value for maximum number of possitive clues for columns
            Do
                stringLine = objReader.ReadLine()
            Loop Until stringLine.Contains("# Maximum number of (possitive) clues for columns")
            stringLine = objReader.ReadLine()
            extractInfo = Replace(stringLine, " ", "")
            Integer.TryParse(extractInfo, maxNumberOfCluesOnColumns)
            ' Close file after getting necessary data
            objReader.Close()
        Catch ex As Exception
            errInfo = ex.Message
        End Try

        ' Get data in the part concerning values of clues
        Dim clueMatrixForRows(0 To numberOfRowsOfPuzzle - 1, 0 To maxNumberOfCluesOnRows - 1) As Integer
        Dim clueMatrixForColumns(0 To maxNumberOfCluesOnColumns - 1, 0 To numberOfColumnsOfPuzzle - 1) As Integer
        Dim clueValue As Integer
        Dim iRow, iColumn, iClue As Integer

        Try
            fileToRead = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileCurrentInterfaceInput
            objReader = New StreamReader(fileToRead)
            stringLine = ""
            ' Read file line by line until the line containing "# Row clues"
            ' (the next lines in the file will contain data of clues for rows)
            Do
                stringLine = objReader.ReadLine()
            Loop Until stringLine.Contains("# Row clues")
            ' The lines after the comment line "# Row clues" and before the comment line "# Column clues"
            ' contains the clues for rows of the puzzle
            iRow = 0
            Do
                stringLine = objReader.ReadLine()
                If Not stringLine.Contains("# Column clues") Then
                    ' Extract and save data to the matrix
                    For iClue = 0 To maxNumberOfCluesOnRows - 1
                        extractInfo = stringLine.Substring(iClue * clsGlobalParameters.nSpacesForSavingClues, clsGlobalParameters.nSpacesForSavingClues)
                        extractInfo = Replace(extractInfo, " ", "")
                        Integer.TryParse(extractInfo, clueValue)
                        clueMatrixForRows(iRow, iClue) = clueValue
                    Next
                    ' Increase the row index after reading each line of the part concerning clues for row
                    iRow = iRow + 1
                End If
            Loop Until stringLine.Contains("# Column clues")
            ' The lines after the comment line "# Column clues" contains the clues for columns of the puzzle
            iColumn = 0
            Do
                stringLine = objReader.ReadLine()
                If Not stringLine.Contains("# Column clues") Then
                    ' Extract and save data to the matrix
                    For iClue = 0 To numberOfColumnsOfPuzzle - 1
                        extractInfo = stringLine.Substring(iClue * clsGlobalParameters.nSpacesForSavingClues, clsGlobalParameters.nSpacesForSavingClues)
                        extractInfo = Replace(extractInfo, " ", "")
                        Integer.TryParse(extractInfo, clueValue)
                        clueMatrixForColumns(iColumn, iClue) = clueValue
                    Next
                    ' Increase the row index after reading each line of the part concerning clues for columns
                    iColumn = iColumn + 1
                End If
            Loop Until stringLine = "" Or stringLine Is Nothing
            ' Close file after getting necessary data
            objReader.Close()
        Catch ex As Exception
            errInfo = ex.Message
        End Try

        ' FROM DATA SAVED IN THE MATRICES, COMPUTE AND SAVE ZPL INPUT DATA TO FILES
        Dim fileToWrite As String
        Dim writeFile As System.IO.TextWriter
        Dim count As Integer

        ' Write data to file puzzle size
        fileToWrite = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileCurrentPuzzleSize
        If System.IO.File.Exists(fileToWrite) = True Then
            System.IO.File.Delete(fileToWrite)
        End If
        Try
            writeFile = New StreamWriter(fileToWrite)
            writeFile.WriteLine("# Size of current Nonogram puzzle")
            writeFile.WriteLine("# Number of rows of Nonogram grid")
            writeFile.WriteLine(numberOfRowsOfPuzzle.ToString())
            writeFile.WriteLine("# Number of columns of Nonogram grid")
            writeFile.WriteLine(numberOfColumnsOfPuzzle.ToString())

            writeFile.Flush()
            writeFile.Close()
            writeFile = Nothing
        Catch ex As IOException
            MsgBox(ex.ToString)
        End Try

        ' Count the number of positive clues for each row
        Dim numberOfPositiveCluesOnRow(0 To numberOfRowsOfPuzzle - 1) As Integer
        For iRow = 0 To numberOfRowsOfPuzzle - 1
            count = 0
            For iClue = 0 To maxNumberOfCluesOnRows - 1
                If clueMatrixForRows(iRow, iClue) > 0 Then
                    count = count + 1
                End If
            Next
            numberOfPositiveCluesOnRow(iRow) = count
        Next
        ' Write data to file number of (positive) clues for each rows
        fileToWrite = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileCurrentCardClueRow
        If System.IO.File.Exists(fileToWrite) = True Then
            System.IO.File.Delete(fileToWrite)
        End If
        Try
            writeFile = New StreamWriter(fileToWrite)
            writeFile.WriteLine("# Row index    Number of clues on row")
            For iRow = 0 To numberOfRowsOfPuzzle - 1
                writeFile.WriteLine((iRow + 1).ToString.PadLeft(clsGlobalParameters.nSpacesForSavingClues) + numberOfPositiveCluesOnRow(iRow).ToString.PadLeft(clsGlobalParameters.nSpacesForSavingClues))
            Next

            writeFile.Flush()
            writeFile.Close()
            writeFile = Nothing
        Catch ex As IOException
            MsgBox(ex.ToString)
        End Try

        ' Count the number of positive clues for each column
        Dim numberOfPositiveCluesOnColumn(0 To numberOfColumnsOfPuzzle - 1) As Integer
        For iColumn = 0 To numberOfColumnsOfPuzzle - 1
            count = 0
            For iClue = 0 To maxNumberOfCluesOnColumns - 1
                If clueMatrixForColumns(iClue, iColumn) > 0 Then
                    count = count + 1
                End If
            Next
            numberOfPositiveCluesOnColumn(iColumn) = count
        Next
        ' Write data to file number of (positive) clues for each column
        fileToWrite = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileCurrentCardClueColumn
        If System.IO.File.Exists(fileToWrite) = True Then
            System.IO.File.Delete(fileToWrite)
        End If
        Try
            writeFile = New StreamWriter(fileToWrite)
            writeFile.WriteLine("# Column index    Number of clues on column")
            For iColumn = 0 To numberOfColumnsOfPuzzle - 1
                writeFile.WriteLine((iColumn + 1).ToString.PadLeft(clsGlobalParameters.nSpacesForSavingClues) + numberOfPositiveCluesOnColumn(iColumn).ToString.PadLeft(clsGlobalParameters.nSpacesForSavingClues))
            Next

            writeFile.Flush()
            writeFile.Close()
            writeFile = Nothing
        Catch ex As IOException
            MsgBox(ex.ToString)
        End Try

        ' Write data to file current information (including coordinate and corresponding block range) of clues for rows
        fileToWrite = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileCurrentInfoClueRow
        If System.IO.File.Exists(fileToWrite) = True Then
            System.IO.File.Delete(fileToWrite)
        End If
        Try
            writeFile = New StreamWriter(fileToWrite)
            writeFile.WriteLine("# Information of clues on rows")
            writeFile.WriteLine("# Row index    Order of clue    Value of clue    Begin range    End range")
            Dim indexBegin As Integer = 0     ' index of the cell that begins the range of the starting cell of a block
            Dim indexEnd As Integer = 0       ' index of the cell that ends the range of the starting cell of a block
            Dim nPCR As Integer               ' variable to keep a value in an array in order to save time of accessing array
            Dim i As Integer                  ' variable for scanning indices of arrays
            Dim str As String                 ' variable to construct the string for each line to be written into file
            Dim nSpaces As Integer = clsGlobalParameters.nSpacesForSavingClues   ' variable to save time of accessing another class
            For iRow = 0 To numberOfRowsOfPuzzle - 1
                nPCR = numberOfPositiveCluesOnRow(iRow)
                If nPCR > 0 Then
                    For iClue = 0 To nPCR - 1
                        ' Compute index of the cell that begins the range of the starting cell of the considering block
                        If iClue = 0 Then
                            indexBegin = 1
                        Else
                            indexBegin = iClue + 1
                            For i = 0 To iClue - 1
                                indexBegin = indexBegin + clueMatrixForRows(iRow, i)
                            Next
                        End If
                        ' Compute index of the cell that ends the range of the starting cell of the considering block
                        indexEnd = numberOfColumnsOfPuzzle + 1 - nPCR + (iClue + 1)
                        For i = iClue To nPCR - 1
                            indexEnd = indexEnd - clueMatrixForRows(iRow, i)
                        Next
                        ' Then write the information corresponding to the considering clue into file
                        str = ""
                        str = str + (iRow + 1).ToString.PadLeft(nSpaces)
                        str = str + (iClue + 1).ToString.PadLeft(nSpaces)
                        str = str + clueMatrixForRows(iRow, iClue).ToString.PadLeft(nSpaces)
                        str = str + indexBegin.ToString.PadLeft(nSpaces)
                        str = str + indexEnd.ToString.PadLeft(nSpaces)
                        writeFile.WriteLine(str)
                    Next
                End If
            Next

            writeFile.Flush()
            writeFile.Close()
            writeFile = Nothing
        Catch ex As IOException
            MsgBox(ex.ToString)
        End Try

        ' Write data to file current information (including coordinate and corresponding block range) of clues for columns
        fileToWrite = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileCurrentInfoClueColumn
        If System.IO.File.Exists(fileToWrite) = True Then
            System.IO.File.Delete(fileToWrite)
        End If
        Try
            writeFile = New StreamWriter(fileToWrite)
            writeFile.WriteLine("# Information of clues on columns")
            writeFile.WriteLine("# Column index    Order of clue    Value of clue    Begin range    End range")
            Dim indexBegin As Integer = 0     ' index of the cell that begins the range of the starting cell of a block
            Dim indexEnd As Integer = 0       ' index of the cell that ends the range of the starting cell of a block
            Dim nPCC As Integer               ' variable to keep a value in an array in order to save time of accessing array
            Dim i As Integer                  ' variable for scanning indices of arrays
            Dim str As String                 ' variable to construct the string for each line to be written into file
            Dim nSpaces As Integer = clsGlobalParameters.nSpacesForSavingClues   ' variable to save time of accessing another class
            For iColumn = 0 To numberOfColumnsOfPuzzle - 1
                nPCC = numberOfPositiveCluesOnColumn(iColumn)
                If nPCC > 0 Then
                    For iClue = 0 To nPCC - 1
                        ' Compute index of the cell that begins the range of the starting cell of the considering block
                        If iClue = 0 Then
                            indexBegin = 1
                        Else
                            indexBegin = iClue + 1
                            For i = 0 To iClue - 1
                                indexBegin = indexBegin + clueMatrixForColumns(i, iColumn)
                            Next
                        End If
                        ' Compute index of the cell that ends the range of the starting cell of the considering block
                        indexEnd = numberOfRowsOfPuzzle + 1 - nPCC + (iClue + 1)
                        For i = iClue To nPCC - 1
                            indexEnd = indexEnd - clueMatrixForColumns(i, iColumn)
                        Next
                        ' Then write the information corresponding to the considering clue into file
                        str = ""
                        str = str + (iColumn + 1).ToString.PadLeft(nSpaces)
                        str = str + (iClue + 1).ToString.PadLeft(nSpaces)
                        str = str + clueMatrixForColumns(iClue, iColumn).ToString.PadLeft(nSpaces)
                        str = str + indexBegin.ToString.PadLeft(nSpaces)
                        str = str + indexEnd.ToString.PadLeft(nSpaces)
                        writeFile.WriteLine(str)
                    Next
                End If
            Next

            writeFile.Flush()
            writeFile.Close()
            writeFile = Nothing
        Catch ex As IOException
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub WriteZimplModelFile()
        ' If the file exists from previous run, then delete it
        Dim fileToWrite As String
        Dim writeFile As System.IO.TextWriter
        fileToWrite = clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileZimplModel
        If System.IO.File.Exists(fileToWrite) = True Then
            System.IO.File.Delete(fileToWrite)
        End If

        ' Write the zimpl model to file
        Try
            writeFile = New StreamWriter(fileToWrite)
            writeFile.WriteLine("# This is zimpl model of IP formulation for Picross puzzles.")
            writeFile.WriteLine("# Copyright @ Xuan Thanh Le, Nguyen Thi Nga, Le Thanh Nga 2019.")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Path to the folder containing data files for zimpl model")
            writeFile.WriteLine("param pathToDataFolder := " + """" + clsGlobalParameters.pathToFolderContainingDataFiles + """" + ";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# File name of input data for current Nonogram puzzle")
            writeFile.WriteLine("param filePuzzleSize := pathToDataFolder + " + """" + clsGlobalParameters.nameOfFileCurrentPuzzleSize + """" + ";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Number of rows of Picross puzzle")
            writeFile.WriteLine("param numberOfRows := read filePuzzleSize as ""1n"" use 1 comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Number of columns of Picross puzzle")
            writeFile.WriteLine("param numberOfColumns := read filePuzzleSize as ""1n"" skip 1 use 1 comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Name of file number of clues on each row")
            writeFile.WriteLine("param fileCardClueRow := pathToDataFolder + " + """" + clsGlobalParameters.nameOfFileCurrentCardClueRow + """" + ";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# List of row names")
            writeFile.WriteLine("set Rows := {read fileCardClueRow as ""<1n>"" comment ""#""};")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Number of clues on each row")
            writeFile.WriteLine("param cardClueOnRow[Rows] := read fileCardClueRow as ""<1n> 2n"" comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Name of file number of clues on each column")
            writeFile.WriteLine("param fileCardClueColumn :=  pathToDataFolder + " + """" + clsGlobalParameters.nameOfFileCurrentCardClueColumn + """" + ";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# List of column names")
            writeFile.WriteLine("set Columns := {read fileCardClueColumn as ""<1n>"" comment ""#""};")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Number of clues on each column")
            writeFile.WriteLine("param cardClueOnColumn[Columns] := read fileCardClueColumn as ""<1n> 2n"" comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Name of file information of clues on rows")
            writeFile.WriteLine("param fileInfoClueRow :=  pathToDataFolder + " + """" + clsGlobalParameters.nameOfFileCurrentInfoClueRow + """" + ";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Row index and position of each row clue")
            writeFile.WriteLine("set ClueOrderOnRow := {read fileInfoClueRow as ""<1n, 2n>"" comment ""#""};")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Value of clues on rows")
            writeFile.WriteLine("param clueValueRow[ClueOrderOnRow]:= read fileInfoClueRow as ""<1n, 2n> 3n"" comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# (Column) index of the first possible starting cell of each block on row")
            writeFile.WriteLine("param startRangeBlockRow[ClueOrderOnRow]:= read fileInfoClueRow as ""<1n, 2n> 4n"" comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# (Column) index of the last possible starting cell of each block on row")
            writeFile.WriteLine("param endRangeBlockRow[ClueOrderOnRow]:= read fileInfoClueRow as ""<1n, 2n> 5n"" comment ""#""; ")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Name of file information of clues on columns")
            writeFile.WriteLine("param fileInfoClueColumn := pathToDataFolder + " + """" + clsGlobalParameters.nameOfFileCurrentInfoClueColumn + """" + "; ")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Column index and position of each column clue")
            writeFile.WriteLine("set ClueOrderOnColumn := {read fileInfoClueColumn as ""<1n, 2n>"" comment ""#""};")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Value of clues on columns")
            writeFile.WriteLine("param clueValueColumn[ClueOrderOnColumn]:= read fileInfoClueColumn as ""<1n, 2n> 3n"" comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# (Row) index of the first possible starting cell of each block on column")
            writeFile.WriteLine("param startRangeBlockColumn[ClueOrderOnColumn]:= read fileInfoClueColumn as ""<1n, 2n> 4n"" comment ""#""; ")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# (Row) index of the last possible starting cell of each block on column")
            writeFile.WriteLine("param endRangeBlockColumn[ClueOrderOnColumn]:= read fileInfoClueColumn as ""<1n, 2n> 5n"" comment ""#"";")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("## VARIABLES")
            writeFile.WriteLine("var x[Rows * Columns] binary;")
            writeFile.WriteLine("var y[ClueOrderOnRow * Columns] binary;")
            writeFile.WriteLine("var z[Rows * ClueOrderOnColumn] binary;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("## CONSTRAINTS ON ROWS")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# If no possitive clues is given for a row, then all cells of that row are blank")
            writeFile.WriteLine("subto RowEmpty:")
            writeFile.WriteLine("	forall <iRow> in Rows with cardClueOnRow[iRow] == 0 do")
            writeFile.WriteLine("		forall <jColumn> in Columns do")
            writeFile.WriteLine("			 x[iRow, jColumn] == 0;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Each block on a row must have unique starting cell")
            writeFile.WriteLine("subto RowUniqueStartBlock:")
            writeFile.WriteLine("	forall <iRow, pBlock> in ClueOrderOnRow do")
            writeFile.WriteLine("		sum <jColumn> in Columns: y[iRow, pBlock, jColumn] == 1;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Restrict the range for the starting cell of each block on each row")
            writeFile.WriteLine("subto RowRestrictRangeBlock:")
            writeFile.WriteLine("	forall <iRow, pBlock> in ClueOrderOnRow do")
            writeFile.WriteLine("		forall <jColumn> in Columns with jColumn < startRangeBlockRow[iRow, pBlock] or jColumn > endRangeBlockRow[iRow, pBlock] do")
            writeFile.WriteLine("			y[iRow, pBlock, jColumn] == 0;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Determine colored cells of each block on each row")
            writeFile.WriteLine("# Consecutive colored cells of block")
            writeFile.WriteLine("subto RowBlockCells:")
            writeFile.WriteLine("	forall <iRow, pBlock> in ClueOrderOnRow do")
            writeFile.WriteLine("		forall <jColumn> in {startRangeBlockRow[iRow, pBlock] .. endRangeBlockRow[iRow, pBlock]} do")
            writeFile.WriteLine("			forall <ell> in {0 .. clueValueRow[iRow, pBlock] - 1} do")
            writeFile.WriteLine("				y[iRow, pBlock, jColumn] <= x[iRow, jColumn + ell];")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Ensure order of blocks on row in forward direction")
            writeFile.WriteLine("subto RowBlockOrderForward:")
            writeFile.WriteLine("	forall <iRow, pBlock> in ClueOrderOnRow with cardClueOnRow[iRow] >= 2 and pBlock <= cardClueOnRow[iRow] - 1 do")
            writeFile.WriteLine("		forall <jColumn> in {startRangeBlockRow[iRow, pBlock] .. endRangeBlockRow[iRow, pBlock]} do")
            writeFile.WriteLine("			y[iRow, pBlock, jColumn] + sum <ell> in {1 .. jColumn + clueValueRow[iRow, pBlock]}: y[iRow, pBlock + 1, ell] <= 1;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# The total number of colored cells on each row must equal the total value of clues given for that row")
            writeFile.WriteLine("subto RowTotalBlackCells:")
            writeFile.WriteLine("	 forall <iRow> in Rows with cardClueOnRow[iRow] >= 1 do")
            writeFile.WriteLine("			sum <jColumn> in Columns: x[iRow, jColumn] == sum <pBlock> in {1 .. cardClueOnRow[iRow]}: clueValueRow[iRow, pBlock];")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("## CONSTRAINTS ON COLUMNS")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# If no possitive clues is given for a column, then all cells of that column are blank")
            writeFile.WriteLine("subto ColumnEmpty:")
            writeFile.WriteLine("	forall <jColumn> in Columns with cardClueOnColumn[jColumn] == 0 do")
            writeFile.WriteLine("		forall <iRow> in Rows do")
            writeFile.WriteLine("			 x[iRow, jColumn] == 0;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Each block on a column must have unique starting cell	")
            writeFile.WriteLine("subto ColumnUniqueStartBlock:")
            writeFile.WriteLine("	forall <jColumn, qBlock> in ClueOrderOnColumn do")
            writeFile.WriteLine("		sum <iRow> in Rows: z[iRow, jColumn, qBlock] == 1;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Restrict the range for the starting cell of each block on each column")
            writeFile.WriteLine("subto ColumnRestrictRangeBlock:")
            writeFile.WriteLine("	forall <jColumn, qBlock> in ClueOrderOnColumn do")
            writeFile.WriteLine("		forall <iRow> in Rows with iRow < startRangeBlockColumn[jColumn, qBlock] or iRow > endRangeBlockColumn[jColumn, qBlock] do")
            writeFile.WriteLine("			z[iRow, jColumn, qBlock] == 0;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Determine colored cells of each block on each column")
            writeFile.WriteLine("# Consecutive colored cells of block")
            writeFile.WriteLine("subto ColumnBlockCells:")
            writeFile.WriteLine("	forall <jColumn, qBlock> in ClueOrderOnColumn do")
            writeFile.WriteLine("		forall <iRow> in {startRangeBlockColumn[jColumn, qBlock] .. endRangeBlockColumn[jColumn, qBlock]} do")
            writeFile.WriteLine("			forall <ell> in {0 .. clueValueColumn[jColumn, qBlock] - 1} do")
            writeFile.WriteLine("				z[iRow, jColumn, qBlock] <= x[iRow + ell, jColumn];")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# Ensure order of blocks on column in top to bottom direction")
            writeFile.WriteLine("subto ColumnBlockOrderForward:")
            writeFile.WriteLine("	forall <jColumn, qBlock> in ClueOrderOnColumn with cardClueOnColumn[jColumn] >= 2 and qBlock <= cardClueOnColumn[jColumn] - 1 do")
            writeFile.WriteLine("		forall <iRow> in {startRangeBlockColumn[jColumn, qBlock] .. endRangeBlockColumn[jColumn, qBlock]} do")
            writeFile.WriteLine("			z[iRow, jColumn, qBlock] + sum <ell> in {1 .. iRow + clueValueColumn[jColumn, qBlock]}: z[ell, jColumn, qBlock + 1] <= 1;")
            writeFile.WriteLine(" ")
            writeFile.WriteLine("# The total number of colored cells on each column must equal the total value of clues given for that column")
            writeFile.WriteLine("subto ColumnTotalBlackCells:")
            writeFile.WriteLine("	 forall <jColumn> in Columns with cardClueOnColumn[jColumn] >= 1 do")
            writeFile.WriteLine("			sum <iRow> in Rows: x[iRow, jColumn] == sum <qBlock> in {1 .. cardClueOnColumn[jColumn]}: clueValueColumn[jColumn, qBlock];")
            
            WriteFile.Flush()
            WriteFile.Close()
            WriteFile = Nothing

        Catch ex As IOException
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub WriteScipSettingFile()
        ' If the file exists from previous run, then delete it
        Dim fileToWrite As String
        Dim writeFile As System.IO.TextWriter
        fileToWrite = clsGlobalParameters.pathToFolderContainingScipSolver + clsGlobalParameters.nameOfFileSettingOfSCIP
        If System.IO.File.Exists(fileToWrite) = True Then
            System.IO.File.Delete(fileToWrite)
        End If

        Try
            writeFile = New StreamWriter(fileToWrite)

            WriteFile.WriteLine("# maximal time in seconds to run")
            WriteFile.WriteLine("# [type: real, range: [0,1.79769313486232e+308], default: 1e+020]")
            writeFile.WriteLine("limits/time = " + clsGlobalParameters.timeLimitSCIP.ToString())

            WriteFile.Flush()
            WriteFile.Close()
            WriteFile = Nothing

        Catch ex As IOException
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub RunZIMPL()
        Dim procStartZimplInfo As New ProcessStartInfo
        Dim Instance As Process

        ' Running ZIMPL to get lp file
        procStartZimplInfo.FileName = clsGlobalParameters.pathToFolderContainingZimpl + clsGlobalParameters.nameOfModelLanguageZIMPL
        procStartZimplInfo.Arguments = " " + clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileZimplModel
        procStartZimplInfo.WindowStyle = ProcessWindowStyle.Hidden

        Instance = Process.Start(procStartZimplInfo)
        Dim zimplIsProcessing As Boolean
        Do
            zimplIsProcessing = Instance.HasExited()
        Loop Until zimplIsProcessing = True
    End Sub

    Private Sub RunSCIP()
        Dim Instance As Process
        Dim fileToDelete As String

        ' Run SCIP solver
        WriteScipSettingFile()

        fileToDelete = clsGlobalParameters.pathToFolderContainingResultFiles + clsGlobalParameters.nameOfFileResultOfSCIP
        If System.IO.File.Exists(fileToDelete) = True Then
            System.IO.File.Delete(fileToDelete)
        End If

        ' Start the process of solving zimpl model by SCIP (note that the version for window of SCIP already included zimpl)
        Dim procScipStartInfo As New ProcessStartInfo
        procScipStartInfo.FileName = clsGlobalParameters.pathToFolderContainingScipSolver + clsGlobalParameters.nameOfSolverSCIP
        ' Running SCIP using setting file
        procScipStartInfo.Arguments = " -s " + clsGlobalParameters.pathToFolderContainingScipSolver + clsGlobalParameters.nameOfFileSettingOfSCIP + " -q -f " + clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileLP + " -l " + clsGlobalParameters.pathToFolderContainingModelFiles + clsGlobalParameters.nameOfFileResultOfSCIP
        ' Running SCIP using default setting
        ' procStartInfo.Arguments = " -q -f " + clsGlobalParameters.PathToFolderContainingModelFiles + NameOfLpFile + " -l " + clsGlobalParameters.PathToFolderContainingModelFiles + NameOfResultFileOfScip
        procScipStartInfo.WindowStyle = ProcessWindowStyle.Hidden
        Instance = Process.Start(procScipStartInfo)
        Dim scipIsProcessing As Boolean
        Do
            scipIsProcessing = Instance.HasExited()
        Loop Until scipIsProcessing = True
    End Sub

    Private Sub PresentSolution()
        ' READ DATA FROM RESULT FILE OF SCIP and SAVE SOLUTION IN A MATRICES
        Dim fileToRead As String
        Dim objReader As StreamReader
        Dim stringLine As String
        Dim errInfo As String = ""

        Dim solutionExistence As Integer
        ' Dim recognizeNonZero As String
        Dim RawData As String = ""

        fileToRead = clsGlobalParameters.pathToFolderContainingResultFiles + clsGlobalParameters.nameOfFileResultOfSCIP
        ' Read file result of SCIP to know solution existence status
        Try
            objReader = New StreamReader(fileToRead)
            stringLine = ""
            stringLine = objReader.ReadLine()
            'MessageBox.Show(fileToRead)
            'MessageBox.Show(stringLine)
            Do
                If Not (stringLine.Contains("optimal solution found") Or stringLine.Contains("time limit reached") Or stringLine.Contains("infeasible")) Then
                    stringLine = objReader.ReadLine()
                    'MessageBox.Show(stringLine)
                    If stringLine.Contains("time limit reached") Then
                        solutionExistence = 2
                        MessageBox.Show("Time limit is reached!", "Solution existence")
                    End If
                    If stringLine.Contains("optimal solution found") Then
                        solutionExistence = 1
                        MessageBox.Show("Puzzle is solved!", "Solution existence")
                    End If
                    If stringLine.Contains("infeasilbe") Then
                        solutionExistence = 0
                        MessageBox.Show("Puzzle has no solution!", "Solution existence")
                    End If
                End If
            Loop Until stringLine.Contains("optimal solution found") Or stringLine.Contains("time limit reached") Or stringLine.Contains("infeasible")

            Do
                stringLine = objReader.ReadLine()
                If stringLine.Contains("x#") Then
                    Dim result As String() = stringLine.Split(New String() {"("}, StringSplitOptions.None)
                    For Each s As String In result
                        If (s.Contains("x#")) Then
                            Dim result_2 As String() = stringLine.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)
                            For Each k As String In result_2
                                If k.Contains("x#") Then
                                    Dim result_3 As String() = k.Split(New String() {"#"}, StringSplitOptions.None)
                                    For Each l As String In result_3
                                        If l <> "x" Then
                                            RawData = RawData + " " + l
                                        End If
                                    Next
                                    RawData = RawData + "|"
                                End If
                            Next
                        End If
                    Next
                    'MessageBox.Show(result)

                End If
            Loop Until stringLine.Contains("y#")

            MessageBox.Show("data here" + RawData)
            Dim _row As Integer = 4
            Dim _col As Integer = 6
            Dim MatrixOutput(3, 5) As Integer
            Dim result_4 As String() = RawData.Split(New String() {"|"}, StringSplitOptions.None)
            For Each o As String In result_4
                Dim count As Integer = 0
                Dim result_5 As String() = o.Split(New String() {" "}, StringSplitOptions.None)
                Dim i, j As Integer
                For Each q As String In result_5
                    count += 1
                    If count = 1 Then
                        i = Integer.Parse(q)
                    End If
                    If count = 2 Then
                        j = Integer.Parse(q)
                    End If
                Next
                MatrixOutput(i - 1, j - 1) = 1
                'MessageBox.Show("aaa" + MatrixOutput(1, 2))
            Next
            MessageBox.Show("sgashs")

            objReader.Close()
        Catch ex As Exception
            errInfo = ex.Message
        End Try



        '' In case that a solution exists, save the information of the solution to an array

        'Try
        '    ObjReader = New StreamReader(fileToRead)
        '    StringLine = ""

        '    ' Get information about gap in case of time limit reached
        '    If solutionExistence = 1 Or solutionExistence = 2 Then
        '        Do
        '            StringLine = ObjReader.ReadLine()
        '        Loop Until StringLine.Contains("objective value:") Or StringLine Is Nothing
        '        ' Convert data from output of scip
        '        Do
        '            StringLine = ObjReader.ReadLine()
        '            If Not (StringLine Is Nothing Or StringLine = "") Then
        '                RecognizeNonZero = StringLine.Substring(0, 2)
        '                ' Handle lines corresponding to variables related to deterministic items
        '                If RecognizeNonZero = "x#" Then
        '                    ' Recognize data fields
        '                    r = StringLine.Substring(2, 1)
        '                    c = StringLine.Substring(4, 1)
        '                    v = StringLine.Substring(6, 1)
        '                    Integer.TryParse(r, row)
        '                    Integer.TryParse(c, col)
        '                    Integer.TryParse(v, val)
        '                    ' Save to an array
        '                    numberOfCells = numberOfCells + 1
        '                    If numberOfCells = 1 Then
        '                        ReDim SudokuSolution(0 To 0)
        '                    Else
        '                        ReDim Preserve SudokuSolution(0 To numberOfCells - 1)
        '                    End If
        '                    SudokuSolution(numberOfCells - 1).row = row
        '                    SudokuSolution(numberOfCells - 1).column = col
        '                    SudokuSolution(numberOfCells - 1).value = val
        '                End If
        '            End If
        '        Loop Until StringLine Is Nothing Or StringLine = "" Or StringLine.Contains("Statistics")
        '    End If
        '    ObjReader.Close()
        'Catch ex As Exception
        '    ErrInfo = ex.Message
        'End Try

        '' SAVE DATA IN THE ARRAY TO SDO FILE
        'Dim fileToWrite As String = clsGlobalParameters.pathToFolderContainingResults + clsGlobalParameters.nameOfFileResultSDO
        'If System.IO.File.Exists(fileToWrite) = True Then
        '    System.IO.File.Delete(fileToWrite)
        'End If
        'Try
        '    Dim writeFile As System.IO.TextWriter = New StreamWriter(fileToWrite)
        '    Dim sdoContent(numberOfCells + 1) As Integer
        '    Dim i As Integer
        '    For i = 0 To numberOfCells - 1
        '        sdoContent((SudokuSolution(i).row - 1) * 9 + SudokuSolution(i).column) = SudokuSolution(i).value
        '    Next
        '    Dim sdoContentString As String = ""
        '    For i = 1 To numberOfCells
        '        sdoContentString = sdoContentString + sdoContent(i).ToString()
        '    Next
        '    writeFile.WriteLine(sdoContentString)

        '    writeFile.Flush()
        '    writeFile.Close()
        '    writeFile = Nothing

        'Catch ex As IOException
        '    MsgBox(ex.ToString)
        'End Try
    End Sub
End Class