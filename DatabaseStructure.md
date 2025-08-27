# Database Structure

The database has the following purposes:
1. Add exercises with descriptions, recommended duration and type. 
2. Create exercise programs.
3. Record logs of exercise session.

It consists of four tables, RecordLogModel, ExerciseModel, ProgramModel and ProgramEntryModel.
ExerciseModel has a many to many relationship with ProgramModel,
so ProgramEntryModel is a join table so ExerciseModel has a one to many relationship with ProgramEntryModel
and ProgramEntryModel has a many to one relationship with ProgramModel.
RecordLogModel has a one to many relationship with ExerciseModel. 

## RecordLogModel
* Id - int, primary key
* ExerciseId - int, required, foreign key - RecordLogModel.ExerciseId = ExerciseModel.Id
* Date - DateTime, required
* Reps - int, required
* Sets - int, required
* Weight - decimal, required 

## ExerciseModel
* Id - int, primary key
* Type - string, required
* Category - string, required
* Exercise - string, required
* DefaultDuration - string, required
* DefaultReps - string, required
* Description - string, required

## ProgramEntryModel 
* Id - int, primary key
* Order - int, required
* ExerciseId - int, required, foreign key - ProgramEntryModel.ExerciseId = ExerciseModel.Id
* ProgramId - int, required, foreign key - ProgramEntryModel.ProgramId = ProgramModel.Id

## ProgramModel
* Id - int, primary key
* Name - string, required

