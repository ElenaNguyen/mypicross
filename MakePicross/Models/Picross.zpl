# This is zimpl model of IP formulation for Picross puzzles.
# Copyright @ Xuan Thanh Le, Nguyen Thi Nga, Le Thanh Nga 2019.
 
# Path to the folder containing data files for zimpl model
param pathToDataFolder := "D:\Picross-projects\PicrossGame\MakePicross\Models\";
 
# File name of input data for current Nonogram puzzle
param filePuzzleSize := pathToDataFolder + "CurrentPuzzleSize.dat";
 
# Number of rows of Picross puzzle
param numberOfRows := read filePuzzleSize as "1n" use 1 comment "#";
 
# Number of columns of Picross puzzle
param numberOfColumns := read filePuzzleSize as "1n" skip 1 use 1 comment "#";
 
# Name of file number of clues on each row
param fileCardClueRow := pathToDataFolder + "CurrentCardClueRow.dat";
 
# List of row names
set Rows := {read fileCardClueRow as "<1n>" comment "#"};
 
# Number of clues on each row
param cardClueOnRow[Rows] := read fileCardClueRow as "<1n> 2n" comment "#";
 
# Name of file number of clues on each column
param fileCardClueColumn :=  pathToDataFolder + "CurrentCardClueColumn.dat";
 
# List of column names
set Columns := {read fileCardClueColumn as "<1n>" comment "#"};
 
# Number of clues on each column
param cardClueOnColumn[Columns] := read fileCardClueColumn as "<1n> 2n" comment "#";
 
# Name of file information of clues on rows
param fileInfoClueRow :=  pathToDataFolder + "CurrentInfoClueRow.dat";
 
# Row index and position of each row clue
set ClueOrderOnRow := {read fileInfoClueRow as "<1n, 2n>" comment "#"};
 
# Value of clues on rows
param clueValueRow[ClueOrderOnRow]:= read fileInfoClueRow as "<1n, 2n> 3n" comment "#";
 
# (Column) index of the first possible starting cell of each block on row
param startRangeBlockRow[ClueOrderOnRow]:= read fileInfoClueRow as "<1n, 2n> 4n" comment "#";
 
# (Column) index of the last possible starting cell of each block on row
param endRangeBlockRow[ClueOrderOnRow]:= read fileInfoClueRow as "<1n, 2n> 5n" comment "#"; 
 
# Name of file information of clues on columns
param fileInfoClueColumn := pathToDataFolder + "CurrentInfoClueColumn.dat"; 
 
# Column index and position of each column clue
set ClueOrderOnColumn := {read fileInfoClueColumn as "<1n, 2n>" comment "#"};
 
# Value of clues on columns
param clueValueColumn[ClueOrderOnColumn]:= read fileInfoClueColumn as "<1n, 2n> 3n" comment "#";
 
# (Row) index of the first possible starting cell of each block on column
param startRangeBlockColumn[ClueOrderOnColumn]:= read fileInfoClueColumn as "<1n, 2n> 4n" comment "#"; 
 
# (Row) index of the last possible starting cell of each block on column
param endRangeBlockColumn[ClueOrderOnColumn]:= read fileInfoClueColumn as "<1n, 2n> 5n" comment "#";
 
## VARIABLES
var x[Rows * Columns] binary;
var y[ClueOrderOnRow * Columns] binary;
var z[Rows * ClueOrderOnColumn] binary;
 
## CONSTRAINTS ON ROWS
 
# If no possitive clues is given for a row, then all cells of that row are blank
subto RowEmpty:
	forall <iRow> in Rows with cardClueOnRow[iRow] == 0 do
		forall <jColumn> in Columns do
			 x[iRow, jColumn] == 0;
 
# Each block on a row must have unique starting cell
subto RowUniqueStartBlock:
	forall <iRow, pBlock> in ClueOrderOnRow do
		sum <jColumn> in Columns: y[iRow, pBlock, jColumn] == 1;
 
# Restrict the range for the starting cell of each block on each row
subto RowRestrictRangeBlock:
	forall <iRow, pBlock> in ClueOrderOnRow do
		forall <jColumn> in Columns with jColumn < startRangeBlockRow[iRow, pBlock] or jColumn > endRangeBlockRow[iRow, pBlock] do
			y[iRow, pBlock, jColumn] == 0;
 
# Determine colored cells of each block on each row
# Consecutive colored cells of block
subto RowBlockCells:
	forall <iRow, pBlock> in ClueOrderOnRow do
		forall <jColumn> in {startRangeBlockRow[iRow, pBlock] .. endRangeBlockRow[iRow, pBlock]} do
			forall <ell> in {0 .. clueValueRow[iRow, pBlock] - 1} do
				y[iRow, pBlock, jColumn] <= x[iRow, jColumn + ell];
 
# Ensure order of blocks on row in forward direction
subto RowBlockOrderForward:
	forall <iRow, pBlock> in ClueOrderOnRow with cardClueOnRow[iRow] >= 2 and pBlock <= cardClueOnRow[iRow] - 1 do
		forall <jColumn> in {startRangeBlockRow[iRow, pBlock] .. endRangeBlockRow[iRow, pBlock]} do
			y[iRow, pBlock, jColumn] + sum <ell> in {1 .. jColumn + clueValueRow[iRow, pBlock]}: y[iRow, pBlock + 1, ell] <= 1;
 
# The total number of colored cells on each row must equal the total value of clues given for that row
subto RowTotalBlackCells:
	 forall <iRow> in Rows with cardClueOnRow[iRow] >= 1 do
			sum <jColumn> in Columns: x[iRow, jColumn] == sum <pBlock> in {1 .. cardClueOnRow[iRow]}: clueValueRow[iRow, pBlock];
 
## CONSTRAINTS ON COLUMNS
 
# If no possitive clues is given for a column, then all cells of that column are blank
subto ColumnEmpty:
	forall <jColumn> in Columns with cardClueOnColumn[jColumn] == 0 do
		forall <iRow> in Rows do
			 x[iRow, jColumn] == 0;
 
# Each block on a column must have unique starting cell	
subto ColumnUniqueStartBlock:
	forall <jColumn, qBlock> in ClueOrderOnColumn do
		sum <iRow> in Rows: z[iRow, jColumn, qBlock] == 1;
 
# Restrict the range for the starting cell of each block on each column
subto ColumnRestrictRangeBlock:
	forall <jColumn, qBlock> in ClueOrderOnColumn do
		forall <iRow> in Rows with iRow < startRangeBlockColumn[jColumn, qBlock] or iRow > endRangeBlockColumn[jColumn, qBlock] do
			z[iRow, jColumn, qBlock] == 0;
 
# Determine colored cells of each block on each column
# Consecutive colored cells of block
subto ColumnBlockCells:
	forall <jColumn, qBlock> in ClueOrderOnColumn do
		forall <iRow> in {startRangeBlockColumn[jColumn, qBlock] .. endRangeBlockColumn[jColumn, qBlock]} do
			forall <ell> in {0 .. clueValueColumn[jColumn, qBlock] - 1} do
				z[iRow, jColumn, qBlock] <= x[iRow + ell, jColumn];
 
# Ensure order of blocks on column in top to bottom direction
subto ColumnBlockOrderForward:
	forall <jColumn, qBlock> in ClueOrderOnColumn with cardClueOnColumn[jColumn] >= 2 and qBlock <= cardClueOnColumn[jColumn] - 1 do
		forall <iRow> in {startRangeBlockColumn[jColumn, qBlock] .. endRangeBlockColumn[jColumn, qBlock]} do
			z[iRow, jColumn, qBlock] + sum <ell> in {1 .. iRow + clueValueColumn[jColumn, qBlock]}: z[ell, jColumn, qBlock + 1] <= 1;
 
# The total number of colored cells on each column must equal the total value of clues given for that column
subto ColumnTotalBlackCells:
	 forall <jColumn> in Columns with cardClueOnColumn[jColumn] >= 1 do
			sum <iRow> in Rows: x[iRow, jColumn] == sum <qBlock> in {1 .. cardClueOnColumn[jColumn]}: clueValueColumn[jColumn, qBlock];
