﻿//Apache2, 2014-2018, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm.Text
{

   
    public partial class InternalTextLayerController
    {
        VisualSelectionRange selectionRange;
        internal bool updateJustCurrentLine = true;
        bool enableUndoHistoryRecording = true;
        DocumentCommandCollection commandHistory;
        //BackGroundTextLineWriter backGroundTextLineWriter;
        TextLineWriter textLineWriter;
        TextEditRenderBox visualTextSurface;
       
#if DEBUG
        debugActivityRecorder _dbugActivityRecorder;
        internal bool dbugEnableTextManRecorder = false;
#endif

        public InternalTextLayerController(
            TextEditRenderBox visualTextSurface,
            EditableTextFlowLayer textLayer)
        {
            this.visualTextSurface = visualTextSurface;
            textLineWriter = new TextLineWriter(textLayer);
            //backGroundTextLineWriter = textLineWriter.GetBackgroundWriter();
            commandHistory = new DocumentCommandCollection(this);
            
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder = new debugActivityRecorder();
                textLineWriter.dbugTextManRecorder = _dbugActivityRecorder;
                throw new NotSupportedException();
                _dbugActivityRecorder.Start(null);
            }
#endif
        }
      
        public bool EnableUndoHistoryRecording
        {
            get
            {
                return enableUndoHistoryRecording;
            }
            set
            {
                enableUndoHistoryRecording = value;
            }
        }


        public void AddCharToCurrentLine(char c)
        {
            updateJustCurrentLine = true;
            bool passRemoveSelectedText = false;
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::AddCharToCurrentLine " + c);
                _dbugActivityRecorder.BeginContext();
            }
#endif
            if (SelectionRange != null)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.WriteInfo(SelectionRange);
                }
#endif
                RemoveSelectedText();
                passRemoveSelectedText = true;
            }
            if (passRemoveSelectedText && c == ' ')
            {
            }
            else
            {
                commandHistory.AddDocAction(
                  new DocActionCharTyping(c, textLineWriter.LineNumber, textLineWriter.CharIndex));
            }

            textLineWriter.AddCharacter(c);
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
        }
        public EditableRun CurrentTextRun
        {
            get
            {
                return textLineWriter.GetCurrentTextRun();
            }
        }

        VisualSelectionRangeSnapShot RemoveSelectedText()
        {
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.WriteInfo("TxLMan::RemoveSelectedText");
                _dbugActivityRecorder.BeginContext();
            }
#endif

            if (selectionRange == null)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.WriteInfo("NO_SEL_RANGE");
                    _dbugActivityRecorder.EndContext();
                }
#endif
                return VisualSelectionRangeSnapShot.Empty;
            }
            else if (!selectionRange.IsValid)
            {
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.WriteInfo("!RANGE_ON_SAME_POINT");
                }
#endif
                CancelSelect();
#if DEBUG
                if (dbugEnableTextManRecorder)
                {
                    _dbugActivityRecorder.EndContext();
                }
#endif
                return VisualSelectionRangeSnapShot.Empty;
            }
            selectionRange.SwapIfUnOrder();
            VisualSelectionRangeSnapShot selSnapshot = selectionRange.GetSelectionRangeSnapshot();
            VisualPointInfo startPoint = selectionRange.StartPoint;
            CurrentLineNumber = startPoint.LineId;
            int preCutIndex = startPoint.LineCharIndex;
            textLineWriter.CharIndex = startPoint.LineCharIndex;
            if (selectionRange.IsOnTheSameLine)
            {
                List<EditableRun> tobeDeleteTextRuns = new List<EditableRun>();
                textLineWriter.CopySelectedTextRuns(selectionRange, tobeDeleteTextRuns);
                if (tobeDeleteTextRuns != null && tobeDeleteTextRuns.Count > 0)
                {

                    commandHistory.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRuns,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));
                    textLineWriter.RemoveSelectedTextRuns(selectionRange);
                    updateJustCurrentLine = true;
                }
            }
            else
            {
                int startPointLindId = startPoint.LineId;
                int startPointCharIndex = startPoint.LineCharIndex;
                List<EditableRun> tobeDeleteTextRuns = new List<EditableRun>();
                textLineWriter.CopySelectedTextRuns(selectionRange, tobeDeleteTextRuns);
                if (tobeDeleteTextRuns != null && tobeDeleteTextRuns.Count > 0)
                {
                    commandHistory.AddDocAction(
                    new DocActionDeleteRange(tobeDeleteTextRuns,
                        selSnapshot.startLineNum,
                        selSnapshot.startColumnNum,
                        selSnapshot.endLineNum,
                        selSnapshot.endColumnNum));
                    textLineWriter.RemoveSelectedTextRuns(selectionRange);
                    updateJustCurrentLine = false;
                    textLineWriter.MoveToLine(startPointLindId);
                    textLineWriter.CharIndex = startPointCharIndex;
                }
            }
            CancelSelect();
            visualTextSurface.NotifyTextContentSizeChanged();
#if DEBUG
            if (dbugEnableTextManRecorder)
            {
                _dbugActivityRecorder.EndContext();
            }
#endif
            return selSnapshot;
        }
        void SplitSelectedText()
        {
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                EditableVisualPointInfo[] newPoints = textLineWriter.SplitSelectedText(selRange);
                if (newPoints != null)
                {
                    selRange.StartPoint = newPoints[0];
                    selRange.EndPoint = newPoints[1];
                    return;
                }
                else
                {
                    selectionRange = null;
                }
            }
        }
        public void SplitCurrentLineIntoNewLine()
        {
            RemoveSelectedText();
            commandHistory.AddDocAction(
                 new DocActionSplitToNewLine(textLineWriter.LineNumber, textLineWriter.CharIndex));
            textLineWriter.SplitToNewLine();
            CurrentLineNumber++;
            updateJustCurrentLine = false;
            visualTextSurface.NotifyTextContentSizeChanged();
        }
        public TextSpanStyle GetFirstTextStyleInSelectedRange()
        {
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                if (selectionRange.StartPoint.TextRun != null)
                {
                    return selectionRange.StartPoint.TextRun.SpanStyle;
                }
                else
                {
                    return TextSpanStyle.Empty;
                }
            }
            else
            {
                return TextSpanStyle.Empty;
            }
        }
        public void DoFormatSelection(TextSpanStyle textStyle)
        {
            int startLineNum = textLineWriter.LineNumber;
            int startCharIndex = textLineWriter.CharIndex;
            SplitSelectedText();
            VisualSelectionRange selRange = SelectionRange;
            if (selRange != null)
            {
                foreach (EditableRun r in selRange.GetPrintableTextRunIter())
                {
                    r.SetStyle(textStyle);
                }

                this.updateJustCurrentLine = selectionRange.IsOnTheSameLine;
                CancelSelect();
                //?
                //CharIndex++;
                //CharIndex--;
            }
        }


        public int CurrentLineCharCount
        {
            get
            {
                return textLineWriter.CharCount;
            }
        }

        public int LineCount
        {
            get
            {
                return textLineWriter.LineCount;
            }
        }
        public int CurrentLineCharIndex
        {
            get
            {
                return textLineWriter.CharIndex;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return textLineWriter.CurrentTextRunCharIndex;
            }
        }
        public int CurrentLineNumber
        {
            get
            {
                return textLineWriter.LineNumber;
            }
            set
            {
                int diff = value - textLineWriter.LineNumber;
                switch (diff)
                {
                    case 0:
                        {
                            return;
                        }
                    case 1:
                        {
                            if (textLineWriter.HasNextLine)
                            {
                                textLineWriter.MoveToNextLine();
                                DoHome();
                            }
                        }
                        break;
                    case -1:
                        {
                            if (textLineWriter.HasPrevLine)
                            {
                                textLineWriter.MoveToPrevLine();
                                DoEnd();
                            }
                        }
                        break;
                    default:
                        {
                            if (diff > 1)
                            {
                                textLineWriter.MoveToLine(value);
                            }
                            else
                            {
                                if (value < -1)
                                {
                                    textLineWriter.MoveToLine(value);
                                }
                                else
                                {
                                    textLineWriter.MoveToLine(value);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public VisualSelectionRange SelectionRange
        {
            get
            {
                return selectionRange;
            }
        }
        public void UpdateSelectionRange()
        {
            if (selectionRange != null)
            {
                selectionRange.UpdateSelectionRange();
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo()
        {
            return textLineWriter.GetCurrentPointInfo();
        }


        public void TryMoveCaretTo(int value, bool backward = false)
        {
            if (textLineWriter.CharIndex < 0 && value < -1)
            {
                if (textLineWriter.HasPrevLine)
                {
                    textLineWriter.MoveToPrevLine();
                    DoEnd();
                }
            }
            else
            {
                int lineLength = textLineWriter.CharCount;
                if (textLineWriter.CharIndex >= lineLength - 1 && value > lineLength - 1)
                {
                    if (textLineWriter.HasNextLine)
                    {
                        textLineWriter.MoveToNextLine();
                    }
                }
                else
                {
                    textLineWriter.CharIndex = value;
                    //check if we can stop at this char or not
                    if (backward)
                    {
                        char nextChar = textLineWriter.NextChar;
                        if (nextChar != '\0' && !CanCaretStopOnThisChar(nextChar))
                        {

                            int tmp_index = value + 1;
                            while ((nextChar != '\0' && !CanCaretStopOnThisChar(nextChar)) && tmp_index > 0)
                            {
                                textLineWriter.CharIndex--;
                                nextChar = textLineWriter.NextChar;
                                tmp_index--;
                            }
                        }
                    }
                    else
                    {
                        char nextChar = textLineWriter.NextChar;
                        if (nextChar != '\0' && !CanCaretStopOnThisChar(nextChar))
                        {
                            int lineCharCount = textLineWriter.CharCount;
                            int tmp_index = value + 1;
                            while ((nextChar != '\0' && !CanCaretStopOnThisChar(nextChar)) && tmp_index < lineCharCount)
                            {
                                textLineWriter.CharIndex++;
                                nextChar = textLineWriter.NextChar;
                                tmp_index++;
                            }
                        }

                    }

                }
            }
        }
        public void TryMoveCaretForward()
        {
            //move caret forward 1 key stroke
            TryMoveCaretTo(textLineWriter.CharIndex + 1);
        }
        public void TryMoveCaretBackward()
        {
            TryMoveCaretTo(textLineWriter.CharIndex - 1, true);
        }
        public int CharIndex
        {
            get
            {
                return textLineWriter.CharIndex;
            }
        }
        public bool IsOnEndOfLine
        {
            get { return textLineWriter.IsOnEndOfLine; }
        }
        public bool IsOnStartOfLine
        {
            get
            {
                return textLineWriter.IsOnStartOfLine;
            }
        }
        public int CurrentCaretHeight
        {
            get
            {
                EditableRun currentRun = this.CurrentTextRun;
                return (currentRun != null) ? currentRun.Height : 14;
            }
        }
        public Point CaretPos
        {
            get
            {
                return this.textLineWriter.CaretPosition;
            }
            set
            {
                if (textLineWriter.LineCount > 0)
                {
                    EditableTextLine line = textLineWriter.GetTextLineAtPos(value.Y);
                    int calculatedLineId = 0;
                    if (line != null)
                    {
                        calculatedLineId = line.LineNumber;
                    }
                    this.CurrentLineNumber = calculatedLineId;
                    this.textLineWriter.CaretXPos = value.X;
                }
            }
        }
        public int GetNextCharacterWidth()
        {
            return textLineWriter.NextCharWidth;
        }
        public void SetCaretPos(int x, int y)
        {
            int j = textLineWriter.LineCount;
            if (j > 0)
            {
                EditableTextLine line = textLineWriter.GetTextLineAtPos(y);
                int calculatedLineId = 0;
                if (line != null)
                {
                    calculatedLineId = line.LineNumber;
                }
                this.CurrentLineNumber = calculatedLineId;
                this.textLineWriter.CaretXPos = x;
            }
        }
        public Rectangle CurrentLineArea
        {
            get
            {
                return textLineWriter.LineArea;
            }
        }
        public Rectangle CurrentParentLineArea
        {
            get
            {
                return textLineWriter.ParentLineArea;
            }
        }
        public bool IsOnFirstLine
        {
            get
            {
                return !textLineWriter.HasPrevLine;
            }
        }

        void JoinWithNextLine()
        {
            textLineWriter.JoinWithNextLine();
            visualTextSurface.NotifyTextContentSizeChanged();
        }
        public void UndoLastAction()
        {
            commandHistory.UndoLastAction();
        }
        public void ReverseLastUndoAction()
        {
            commandHistory.ReverseLastUndoAction();
        }
    }
}