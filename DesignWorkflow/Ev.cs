using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DataFactory;
using CDTDatabase;
using DesignWorkflow;
namespace Designflow
{
    public partial class Ev : Form
    {
        public enum State { None, Drawtask, DrawAction, SelectTask, SelectAction, MovingTask, MovingAction, ResizeTask };
        private State _state;
        public State state
        {

            set
            {
                if (_state == State.MovingAction && value != State.MovingAction)
                {
                }
                _state = value;

            }
            get { return _state; }
        }
        public List<Task> lTask = new List<Task>();
        public List<Action> lAction = new List<Action>();
        Task selectedTask;
        Action selecteddAction;
        int linewidth = 1;
        private Bitmap b;

        Color BgC;
        Font font = new Font("Time New Roman", 10);
        Brush brushCl;
        Brush brush = new SolidBrush(Color.Blue);
        Brush brushSl = new SolidBrush(Color.Red);
        Action SelectedAction
        {
            get {
                return selecteddAction;
            }
            set
            {
                selecteddAction = value;
                if (selecteddAction != null)
                {

                    DrawAll();
                    DrawAction(selecteddAction, Color.Red);
                    tActionName.Text = selecteddAction.Name;
                    tCondition.Text = selecteddAction.Condition;
                    tShowCond.Text = selecteddAction.ShowCond;
                    cbAuto.Checked = selecteddAction.AutoDo;
                    cbRefresh.Checked = selecteddAction.isRefresh;
                    pIco.Image = selecteddAction.Icon;
                    tConfirm.Text = selecteddAction.Confirm;
                    tMessage.Text = selecteddAction.Message;
                    ckDoconfig.Checked = selecteddAction.DoconfigData;
                }
                else
                {
                    state = State.None;
                }
            }
        }
        WF wF;
        private Task SelectedTask
        {
            get { return selectedTask; }
            set
            {
                selectedTask = value;
                if (selectedTask != null)
                {
                    tTaskLabel.Text = selectedTask.Label;
                    dbBegin.Checked = selectedTask.isBegin;
                    dbCancel.Checked = selectedTask.isCancel;
                    dbEnd.Checked = selectedTask.isEnd;
                    pTIcon.Image = selectedTask.Icon;
                    tApp.Text = selectedTask.ApprovedStt.ToString();
                    DrawAll();
                    DrawTask(selectedTask, Color.Red);

                }
                else
                {
                    // state = State.None;
                    DrawAll();
                }
            }
        }

        public Ev(CDTData _data)
        {
            InitializeComponent();
            state = State.None;
            Pic.MouseDown += new MouseEventHandler(Gr_MouseDown);
            Pic.MouseUp += new MouseEventHandler(Gr_MouseUp);
            Pic.MouseMove += new MouseEventHandler(Gr_MouseMove);
            BgC = Pic.BackColor;
            b = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            this.Resize += new EventHandler(this_Resize);
            Pic.Paint += new PaintEventHandler(this_Paint);
            this.autoredraw = true;
            Pic.MouseClick += new MouseEventHandler(Ev_MouseClick);
            brushCl = new SolidBrush(BgC);
            this.KeyUp += new KeyEventHandler(Ev_KeyUp);
            this.tTaskLabel.KeyUp += new KeyEventHandler(tTaskLabel_KeyUp);
            wF = new WF(_data.DrTable["sysTableID"].ToString());
            lTask = wF.lTask;
            lAction = wF.lAction;
            DrawAll();
            tWFName.Text = wF.WFName;
        }
        public Ev()
        {
            InitializeComponent();
            state = State.None;
            Pic.MouseDown += new MouseEventHandler(Gr_MouseDown);
            Pic.MouseUp += new MouseEventHandler(Gr_MouseUp);
            Pic.MouseMove += new MouseEventHandler(Gr_MouseMove);
            BgC = Pic.BackColor;
            b = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            this.Resize += new EventHandler(this_Resize);
            Pic.Paint += new PaintEventHandler(this_Paint);
            this.autoredraw = true;
            Pic.MouseClick += new MouseEventHandler(Ev_MouseClick);
            brushCl = new SolidBrush(BgC);
            this.KeyUp += new KeyEventHandler(Ev_KeyUp);
            this.tTaskLabel.KeyUp += new KeyEventHandler(tTaskLabel_KeyUp);
            wF = new WF();
        }


        List<Task> lTaskDelete = new List<Task>();
        List<Action> lActionDelete = new List<Action>();
        void Ev_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (selecteddAction != null)
                {
                    ClearAction(selecteddAction, false);
                    lAction.Remove(selecteddAction);
                    lActionDelete.Add(selecteddAction);
                    selecteddAction = null;
                    state = State.None;

                }
                if (selectedTask != null)
                {
                    ClearTask(selectedTask, true);
                    for (int i = 0; i < lAction.Count; i++)
                    {
                        Action A = lAction[i];
                        if (A.BT == selectedTask)
                        {
                            ClearAction(A, false);
                            lAction.Remove(A);
                            lActionDelete.Add(A);
                            i--;
                        }
                        else if (A.ET == selectedTask)
                        {
                            ClearAction(A, false);
                            lAction.Remove(A);
                            lActionDelete.Add(A);
                            i--;
                        }
                    }
                    lTask.Remove(selectedTask);
                    lTaskDelete.Add(selectedTask);
                    selectedTask = null;
                    state = State.None;
                }
            }
        }

        void Ev_MouseClick(object sender, MouseEventArgs e)
        {
            if ((state == State.None || state == State.SelectTask || state == State.MovingAction || state == State.SelectAction) && e.Button == MouseButtons.Left)
            {
                findTask(e.X, e.Y);
                if (SelectedTask == null)
                    findAction(e.X, e.Y);
                if (selecteddAction == null && selectedTask == null) state = State.None;
            }

        }

        private void findAction(int X, int Y)
        {
            foreach (Action A in lAction)
            {
                for (int i = 1; i < A.P.Count; i++)
                {
                    Point P1 = A.P[i - 1];
                    Point P2 = A.P[i];
                    if (P1.X == P2.X && P1.X - linewidth - 5 < X && P1.X + linewidth + 5 > X && (P1.Y - Y) * (P2.Y - Y) < 0)
                    {
                        SelectedAction = A;
                        if (state != State.MovingAction)
                            state = State.SelectAction;
                        return;
                    }
                    if (P1.Y == P2.Y && P1.Y - linewidth - 5 < Y && P1.Y + linewidth + 5 > Y && (P1.X - X) * (P2.X - X) < 0)
                    {
                        SelectedAction = A;
                        if (state != State.MovingAction)
                            state = State.SelectAction;
                        return;
                    }
                }
            }
            SelectedAction = null;
        }
        private void findActiveLine4Action(Action A, int X, int Y)
        {
            for (int i = 1; i < A.P.Count; i++)
            {
                Point P1 = A.P[i - 1];
                Point P2 = A.P[i];
                if (P1.X == P2.X && P1.X - linewidth - 5 < X && P1.X + linewidth + 5 > X && (P1.Y - Y) * (P2.Y - Y) < 0)
                {
                    A.ActiveLine = new Point[] { P1, P2 };
                    state = State.MovingAction;
                    return;
                }
                if (P1.Y == P2.Y && P1.Y - linewidth - 5 < Y && P1.Y + linewidth + 5 > Y && (P1.X - X) * (P2.X - X) < 0)
                {
                    A.ActiveLine = new Point[] { P1, P2 };
                    state = State.MovingAction;
                    return;
                }
                A.ActiveLine = null;
            }
        }
        private void EndMoveTask(Task T)
        {
            foreach (Action A in lAction)
            {
                if (A.BT == T)
                {
                    ClearAction(A, false);
                    //Lấy Vị trí khởi tạo
                    switch (A.BPName)
                    {
                        case ConnectPoint.TopPoint: A.BPoint = A.BT.TPoint; break;
                        case ConnectPoint.BotPoint: A.BPoint = A.BT.BPoint; break;
                        case ConnectPoint.LeftPoint: A.BPoint = A.BT.LPoint; break;
                        case ConnectPoint.RightPoint: A.BPoint = A.BT.RPoint; break;
                    }
                    setType4Action(A);
                    SetPoint4Action(A);
                    A.isDesign = true;
                    DrawAction(A, false);
                }
                if (A.ET == T)
                {
                    ClearAction(A, false);
                    //Lấy vị trí kết thúc
                    switch (A.EPName)
                    {
                        case ConnectPoint.TopPoint: A.EPoint = A.ET.TPoint; break;
                        case ConnectPoint.BotPoint: A.EPoint = A.ET.BPoint; break;
                        case ConnectPoint.LeftPoint: A.EPoint = A.ET.LPoint; break;
                        case ConnectPoint.RightPoint: A.EPoint = A.ET.RPoint; break;
                    }
                    setType4Action(A);
                    A.isDesign = true;
                    SetPoint4Action(A);
                    DrawAction(A, false);
                }
            }
        }
        private void SetBeginPoint(Action A, Point P)
        {

        }
        private void SetBeginPoint(Action A)
        {

        }
        private void SetEndPoint(Action A)
        {

        }
        private void findTask(int X, int Y)
        {
            foreach (Task T in lTask)
            {
                if ((T.P.X - linewidth - 5 < X && T.P.X + linewidth + 5 > X) || (T.P.X + T.w - linewidth - 5 < X && T.P.X + T.w + linewidth + 5 > X))
                {
                    if (T.P.Y - linewidth - 5 < Y && T.P.Y + T.h + linewidth + 5 > Y)
                    {
                        SelectedTask = T;
                        state = State.SelectTask;
                        return;
                    }
                }
                if ((T.P.Y - linewidth - 5 < Y && T.P.Y + linewidth + 5 > Y) || (T.P.Y + T.h - linewidth - 5 < Y && T.P.Y + T.h + linewidth + 5 > Y))
                {
                    if (T.P.X - linewidth - 5 < X && T.P.X + T.w + linewidth + 5 > X)
                    {
                        SelectedTask = T;
                        state = State.SelectTask;
                        return;
                    }
                }
            }
            SelectedTask = null;

        }
        private Task findTask4Action(int X, int Y)
        {
            double Dis = 99999999999;
            Task ftask = null;
            foreach (Task T in lTask)
            {
                double dis = Math.Pow((Math.Pow((X - T.CPoint.X), 2) + Math.Pow((Y - T.CPoint.Y), 2)), 0.5);
                if (dis < Dis)
                {
                    Dis = dis;
                    ftask = T;
                }

            }

            return ftask;
        }
        private void findBeginPoint4Action(Action A, int X, int Y)
        {
            Task ftask = A.BT;
            if (ftask != null)
            {
                double dis1 = Math.Pow((Math.Pow((X - ftask.TPoint.X), 2) + Math.Pow((Y - ftask.TPoint.Y), 2)), 0.5);
                A.BPName = ConnectPoint.TopPoint;
                double dis2 = Math.Pow((Math.Pow((X - ftask.BPoint.X), 2) + Math.Pow((Y - ftask.BPoint.Y), 2)), 0.5);
                if (dis2 < dis1) A.BPName = ConnectPoint.BotPoint;
                double dis3 = Math.Pow((Math.Pow((X - ftask.LPoint.X), 2) + Math.Pow((Y - ftask.LPoint.Y), 2)), 0.5);
                if (dis3 < dis2 && dis3 < dis1) A.BPName = ConnectPoint.LeftPoint;
                double dis4 = Math.Pow((Math.Pow((X - ftask.RPoint.X), 2) + Math.Pow((Y - ftask.RPoint.Y), 2)), 0.5);
                if (dis4 < dis2 && dis4 < dis1 && dis4 < dis3) A.BPName = ConnectPoint.RightPoint;
                switch (A.BPName)
                {
                    case ConnectPoint.TopPoint: A.BPoint = A.BT.TPoint; A.type = 0; break;
                    case ConnectPoint.BotPoint: A.BPoint = A.BT.BPoint; A.type = 1; break;
                    case ConnectPoint.LeftPoint: A.BPoint = A.BT.LPoint; A.type = 2; break;
                    case ConnectPoint.RightPoint: A.BPoint = A.BT.RPoint; A.type = 3; break;
                }
            }
        }
        private void findEndPoint4Action(Action A, int X, int Y)
        {
            Task ftask = A.ET;
            if (ftask != null)
            {
                double dis1 = Math.Pow((Math.Pow((X - ftask.TPoint.X), 2) + Math.Pow((Y - ftask.TPoint.Y), 2)), 0.5);
                A.EPName = ConnectPoint.TopPoint;
                double dis2 = Math.Pow((Math.Pow((X - ftask.BPoint.X), 2) + Math.Pow((Y - ftask.BPoint.Y), 2)), 0.5);
                if (dis2 < dis1) A.EPName = ConnectPoint.BotPoint;
                double dis3 = Math.Pow((Math.Pow((X - ftask.LPoint.X), 2) + Math.Pow((Y - ftask.LPoint.Y), 2)), 0.5);
                if (dis3 < dis2 && dis3 < dis1) A.EPName = ConnectPoint.LeftPoint;
                double dis4 = Math.Pow((Math.Pow((X - ftask.RPoint.X), 2) + Math.Pow((Y - ftask.RPoint.Y), 2)), 0.5);
                if (dis4 < dis2 && dis4 < dis1 && dis4 < dis3) A.EPName = ConnectPoint.RightPoint;
            }
            setType4Action(A);
        }
        private void setType4Action(Action A)
        {
            switch (A.EPName)
            {
                case ConnectPoint.TopPoint:
                    A.EPoint = A.ET.TPoint;
                    switch (A.BPName)
                    {
                        case ConnectPoint.TopPoint: A.type = 0; break;
                        case ConnectPoint.BotPoint: A.type = 5; ; break;
                        case ConnectPoint.LeftPoint: A.type = 7; break;
                        case ConnectPoint.RightPoint: A.type = 7; break;
                    }
                    break;
                case ConnectPoint.BotPoint: A.EPoint = A.ET.BPoint;
                    switch (A.BPName)
                    {
                        case ConnectPoint.TopPoint: A.type = 5; break;
                        case ConnectPoint.BotPoint: A.type = 1; ; break;
                        case ConnectPoint.LeftPoint: A.type = 7; break;
                        case ConnectPoint.RightPoint: A.type = 7; break;
                    } break;
                case ConnectPoint.LeftPoint: A.EPoint = A.ET.LPoint;
                    switch (A.BPName)
                    {
                        case ConnectPoint.TopPoint: A.type = 6; break;
                        case ConnectPoint.BotPoint: A.type = 6; ; break;
                        case ConnectPoint.LeftPoint: A.type = 2; break;
                        case ConnectPoint.RightPoint: A.type = 4; break;
                    } break;
                case ConnectPoint.RightPoint: A.EPoint = A.ET.RPoint;
                    switch (A.BPName)
                    {
                        case ConnectPoint.TopPoint: A.type = 6; break;
                        case ConnectPoint.BotPoint: A.type = 6; ; break;
                        case ConnectPoint.LeftPoint: A.type = 4; break;
                        case ConnectPoint.RightPoint: A.type = 3; break;
                    } break;
            }
        }

        int mouseX = 0;
        int mouseY = 0;
        void Gr_MouseDown(object sender, MouseEventArgs e)
        {
            if (state == State.Drawtask)
            {
                Task curTast = new Task();
                curTast.P = new Point(e.X, e.Y);
                curTast.h = 1;
                curTast.w = 1;
                selectedTask = curTast;
                // DrawTask(curTast);
            }
            if (selectedTask != null && Cursor == Cursors.SizeAll)
            {
                state = State.MovingTask;
                mouseX = e.X;
                mouseY = e.Y;
            }
            if (SelectedTask != null && state != State.Drawtask)
            {
                Task T = selectedTask;
                if ((T.P.X + T.w - linewidth * 5 < e.X && T.P.X + T.w + linewidth * 5 > e.X) && (T.P.Y + T.h - linewidth * 5 < e.Y && T.P.Y + T.h + linewidth * 5 > e.Y))
                {
                    state = State.ResizeTask;
                    Cursor = Cursors.SizeNWSE;
                }
            }
            if (state == State.DrawAction && e.Button == MouseButtons.Left)
            {
                Action crAction = new Action();
                crAction.BT = findTask4Action(e.X, e.Y);
                findBeginPoint4Action(crAction, e.X, e.Y);
                SetPoint4Action(crAction);
                if (crAction.BT == null) return;
                selecteddAction = crAction;
            }
            if (state == State.MovingAction && e.Button == MouseButtons.Left)
            {

                mouseX = e.X;
                mouseY = e.Y;
            }
        }

        void Gr_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == State.Drawtask)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Task curTast = selectedTask;
                    if (curTast == null) return;
                    ClearTask(curTast, false);
                    curTast.w = e.X - curTast.P.X;
                    curTast.h = e.Y - curTast.P.Y;
                    DrawTask(curTast, false);

                }

            }
            if (SelectedTask != null)
            {
                Task T = selectedTask;
                if ((T.P.X + T.w - linewidth < e.X && T.P.X + T.w + linewidth > e.X) && (T.P.Y + T.h - linewidth < e.Y && T.P.Y + T.h + linewidth > e.Y))
                {
                    Cursor = Cursors.SizeNWSE;
                }
                else if (T.P.X < e.X && T.P.X + T.w > e.X && T.P.Y < e.Y && T.P.Y + T.h > e.Y)
                {
                    Cursor = Cursors.SizeAll;

                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            if (state == State.MovingTask && e.Button == MouseButtons.Left)
            {
                Task T = selectedTask;
                ClearTask(T, true);
                T.P.X = T.P.X + e.X - mouseX;
                T.P.Y = T.P.Y + e.Y - mouseY;
                DrawTask(T, true);
                EndMoveTask(T);
                mouseX = e.X;
                mouseY = e.Y;
            }
            if (state == State.ResizeTask && e.Button == MouseButtons.Left)
            {
                Task T = selectedTask;
                ClearTask(T, true);
                if (e.X - T.P.X > 30) T.w = e.X - T.P.X;
                else T.w = 30;
                if (e.Y - T.P.Y > 30) T.h = e.Y - T.P.Y;
                else T.h = 30;
                DrawTask(T, true);
                EndMoveTask(T);
            }
            if (state == State.DrawAction && e.Button == MouseButtons.Left)
            {
                if (selecteddAction == null)
                    return;
                ClearAction(selecteddAction, false);
                selecteddAction.EPoint = new Point(e.X, e.Y);
                SetPoint4Action(selecteddAction);
                DrawAction(selecteddAction, false);
                selecteddAction.isDesign = true;
            }
            if ((state == State.SelectAction || state == State.MovingAction) && e.Button == MouseButtons.None)
            {
                findActiveLine4Action(selecteddAction, e.X, e.Y);
                if (selecteddAction.ActiveLine == null)
                {
                    Cursor = Cursors.Default;
                    return;

                }
                state = State.MovingAction;
                if (selecteddAction.ActiveLine[0].X == selecteddAction.ActiveLine[1].X) Cursor = Cursors.SizeWE;
                if (selecteddAction.ActiveLine[0].Y == selecteddAction.ActiveLine[1].Y) Cursor = Cursors.SizeNS;
            }
            if (state == State.MovingAction && e.Button == MouseButtons.Left)
            {
                if (selecteddAction == null)
                {
                    state = State.None;
                    return;
                }
                if (selecteddAction.ActiveLine == null)
                {
                    state = State.None;
                    return;
                }
                ClearAction(selecteddAction, false);
                for (int i = 0; i < selecteddAction.P.Count - 1; i++)
                {
                    if (selecteddAction.P[i] == selecteddAction.ActiveLine[0] && selecteddAction.P[i + 1] == selecteddAction.ActiveLine[1])
                    {
                        if (selecteddAction.ActiveLine[0].X == selecteddAction.ActiveLine[1].X)
                        {
                            selecteddAction.P[i] = new Point(e.X, selecteddAction.P[i].Y);
                            selecteddAction.P[i + 1] = new Point(e.X, selecteddAction.P[i + 1].Y);
                        }
                        if (selecteddAction.ActiveLine[0].Y == selecteddAction.ActiveLine[1].Y)
                        {
                            selecteddAction.P[i] = new Point(selecteddAction.P[i].X, e.Y); ;
                            selecteddAction.P[i + 1] = new Point(selecteddAction.P[i + 1].X, e.Y); ;
                        }
                        selecteddAction.ActiveLine = new Point[] { selecteddAction.P[i], selecteddAction.P[i + 1] };
                        break;
                    }
                }
                DrawAction(selecteddAction, Color.Red);
            }
        }


        void Gr_MouseUp(object sender, MouseEventArgs e)
        {
            if (state == State.Drawtask)
            {

                Task curTast = selectedTask;
                if (curTast == null) return;

                curTast.w = e.X - curTast.P.X;
                curTast.h = e.Y - curTast.P.Y;
                lTask.Add(curTast);
                // DrawAll();
                SelectedTask = curTast;


                state = State.SelectTask;


            }
            if (state == State.ResizeTask && e.Button == MouseButtons.Left)
            {
                if (e.X - selectedTask.P.X > 30) selectedTask.w = e.X - selectedTask.P.X;
                else selectedTask.w = 30;
                if (e.Y - selectedTask.P.Y > 30) selectedTask.h = e.Y - selectedTask.P.Y;
                else selectedTask.h = 30;
                DrawAll();
                SelectedTask = selectedTask;
                state = State.SelectTask;
                Cursor = Cursors.Default;
            }
            if (state == State.MovingTask && e.Button == MouseButtons.Left)
            {
                DrawAll();
                SelectedTask = selectedTask;
                EndMoveTask(selectedTask);
                state = State.SelectTask;
                Cursor = Cursors.Default;
            }
            if (state == State.DrawAction && e.Button == MouseButtons.Left)
            {

                if (selecteddAction == null) return;
                if (selecteddAction.ET == null)
                {
                    selecteddAction.ET = findTask4Action(e.X, e.Y);
                }
                if (selecteddAction.ET == null) return;
                findEndPoint4Action(selecteddAction, e.X, e.Y);
                SetPoint4Action(selecteddAction);
                lAction.Add(selecteddAction);
                //DrawAll();
                SelectedAction = selecteddAction;
                selecteddAction.isDesign = false;
                state = State.SelectAction;
            }
            if (state == State.MovingAction && e.Button == MouseButtons.Left)
            {
                if (selecteddAction == null)
                {
                    state = State.None;
                    Cursor = Cursors.Default;
                    return;
                }
                if (selecteddAction.ActiveLine == null)
                {
                    state = State.None;
                    Cursor = Cursors.Default;
                    return;
                }
                ClearAction(selecteddAction, false);
                for (int i = 0; i < selecteddAction.P.Count - 1; i++)
                {
                    if (selecteddAction.P[i] == selecteddAction.ActiveLine[0])
                    {
                        if (selecteddAction.ActiveLine[0].X == selecteddAction.ActiveLine[1].X)
                        {
                            selecteddAction.P[i] = new Point(e.X, selecteddAction.P[i].Y);
                            selecteddAction.P[i + 1] = new Point(e.X, selecteddAction.P[i + 1].Y);
                        }
                        if (selecteddAction.ActiveLine[0].Y == selecteddAction.ActiveLine[1].Y)
                        {
                            selecteddAction.P[i] = new Point(selecteddAction.P[i].X, e.Y); ;
                            selecteddAction.P[i + 1] = new Point(selecteddAction.P[i + 1].X, e.Y); ;
                        }
                        selecteddAction.ActiveLine = new Point[] { selecteddAction.P[i], selecteddAction.P[i + 1] };
                        break;
                    }
                }
                DrawAction(selecteddAction, Color.Red);
            }
        }

        private void DrawAll()
        {
            Graphics graph = Pic.CreateGraphics();
            graph.Clear(BgC);
            Graphics graph1 = this.CreateGraphics();
            graph1.Clear(BgC);
            foreach (Task T in lTask)
            {
                DrawTask(T, true);

            }
            foreach (Action A in lAction)
            {
                DrawAction(A, true);

            }
        }
        private void SetPoint4Action(Action A)
        {
            Point X1; Point X2; int y; int x;
            switch (A.type)
            {
                case 0:

                    y = Math.Min(A.BPoint.Y, A.EPoint.Y) - 50;
                    X1 = new Point(A.BPoint.X, y);
                    X2 = new Point(A.EPoint.X, y);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(X2); A.P.Add(A.EPoint);
                    break;
                case 1:

                    y = Math.Max(A.BPoint.Y, A.EPoint.Y) + 50;
                    X1 = new Point(A.BPoint.X, y);
                    X2 = new Point(A.EPoint.X, y);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(X2); A.P.Add(A.EPoint);
                    break;
                case 2:

                    x = Math.Min(A.BPoint.X, A.EPoint.X) - 50;
                    X1 = new Point(x, A.BPoint.Y);
                    X2 = new Point(x, A.EPoint.Y);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(X2); A.P.Add(A.EPoint);
                    break;
                case 3:

                    x = Math.Max(A.BPoint.X, A.EPoint.X) + 50;
                    X1 = new Point(x, A.BPoint.Y);
                    X2 = new Point(x, A.EPoint.Y);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(X2); A.P.Add(A.EPoint);
                    break;
                case 4:
                    X1 = new Point((A.BPoint.X + A.EPoint.X) / 2, A.BPoint.Y);
                    X2 = new Point((A.BPoint.X + A.EPoint.X) / 2, A.EPoint.Y);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(X2); A.P.Add(A.EPoint);
                    break;
                case 5:
                    X1 = new Point(A.BPoint.X, (A.BPoint.Y + A.EPoint.Y) / 2);
                    X2 = new Point(A.EPoint.X, (A.BPoint.Y + A.EPoint.Y) / 2);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(X2); A.P.Add(A.EPoint);
                    break;

                case 6:
                    X1 = new Point(A.BPoint.X, A.EPoint.Y);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(A.EPoint);
                    break;
                case 7:
                    X1 = new Point(A.EPoint.X, A.BPoint.Y);
                    A.P.Clear(); A.P.Add(A.BPoint); A.P.Add(X1); A.P.Add(A.EPoint);
                    break;
                case 8:

                    break;
                case 9:

                    break;
            }
        }
        private void DrawAction(Action A, bool isText, Pen pen)
        {



            Graphics graph = Pic.CreateGraphics();
            Graphics graph1 = this.CreateGraphics();
            for (int i = 1; i < A.P.Count; i++)
            {
                if (i == A.P.Count - 1)
                {
                    // pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    int des = 0;
                    graph.DrawLine(pen, A.P[i - 1], A.P[i]);
                    graph1.DrawLine(pen, A.P[i - 1], A.P[i]);
                    if (A.P[i - 1].X == A.P[i].X)
                    {
                        if (A.P[i - 1].Y > A.P[i].Y) des = 0;
                        else des = 1;
                    }
                    else if (A.P[i - 1].Y == A.P[i].Y)
                    {
                        if (A.P[i - 1].X > A.P[i].X) des = 2;
                        else des = 3;
                    }
                    DrawArrow(pen, A.P[i], des);
                }
                else
                {
                    graph.DrawLine(pen, A.P[i - 1], A.P[i]);
                    graph1.DrawLine(pen, A.P[i - 1], A.P[i]);
                }
                if (pen.Color == Color.Red || pen.Color == this.BgC)
                {
                    if (A.P[i].X == A.P[i - 1].X)
                    {
                        graph.DrawRectangle(pen, new Rectangle(new Point(A.P[i].X - 2, (A.P[i].Y + A.P[i - 1].Y) / 2), new Size(4, 4)));
                    }
                    else if (A.P[i].Y == A.P[i - 1].Y)
                    {
                        graph.DrawRectangle(pen, new Rectangle(new Point((A.P[i - 1].X + A.P[i].X) / 2, A.P[i].Y - 2), new Size(4, 4)));
                    }
                    if (i == 1) graph.DrawRectangle(pen, new Rectangle(new Point(A.P[i - 1].X - 2, A.P[i - 1].Y - 2), new Size(4, 4)));
                }
            }
        }
        private void ClearAction(Action A, bool isText)
        {
            Pen pen = new Pen(brushCl, linewidth);
            DrawAction(A, false, pen);
        }
        private void DrawArrow(Pen pen, Point P, int des)
        {
            Graphics graph = Pic.CreateGraphics();
            Graphics graph1 = this.CreateGraphics();
            for (int i = -4; i < 5; i++)
            {
                switch (des)
                {
                    case 0: graph.DrawLine(pen, P.X + i, P.Y + 3 + Math.Abs(i), P.X + i, P.Y + Math.Abs(i)); break;
                    case 1: graph.DrawLine(pen, P.X + i, P.Y - 3 - Math.Abs(i), P.X + i, P.Y - Math.Abs(i)); break;
                    case 2: graph.DrawLine(pen, P.X + 3 + Math.Abs(i), P.Y + i, P.X + Math.Abs(i), P.Y + i); break;
                    case 3: graph.DrawLine(pen, P.X - 3 - Math.Abs(i), P.Y + i, P.X - Math.Abs(i), P.Y + i); break;

                }
                switch (des)
                {
                    case 0: graph1.DrawLine(pen, P.X + i, P.Y + 3 + Math.Abs(i), P.X + i, P.Y + Math.Abs(i)); break;
                    case 1: graph1.DrawLine(pen, P.X + i, P.Y - 3 - Math.Abs(i), P.X + i, P.Y - Math.Abs(i)); break;
                    case 2: graph1.DrawLine(pen, P.X + 3 + Math.Abs(i), P.Y + i, P.X + Math.Abs(i), P.Y + i); break;
                    case 3: graph1.DrawLine(pen, P.X - 3 - Math.Abs(i), P.Y + i, P.X - Math.Abs(i), P.Y + i); break;

                }
            }
        }
        private void DrawAction(Action A, bool isText)
        {
            Brush brush1 = new SolidBrush(Color.Green);
            Brush brush2 = new SolidBrush(Color.DarkOrange);
            Pen pen;
            if (A.Command != null && A.Command != string.Empty)
                pen = new Pen(brush1, linewidth);
            else if (A.AfterUpdateCommand != null && A.AfterUpdateCommand != string.Empty)
                pen = new Pen(brush2, linewidth);
            else
                pen = new Pen(brush, linewidth);
            DrawAction(A, false, pen);

        }
        private void DrawAction(Action A, Color Cl)
        {
            Pen pen = new Pen(Cl, linewidth);

            DrawAction(A, false, pen);
        }


        private void ClearTask(Task T, bool isText)
        {
            Pen pen = new Pen(brushCl, linewidth);

            Graphics graph = Pic.CreateGraphics();
            graph.DrawRectangle(pen, new Rectangle(T.P, new Size(T.w, T.h)));
            if (isText) graph.DrawString(T.Label, font, brushCl, new PointF(T.P.X + T.w / 2 - 30, T.P.Y + T.h / 2 - 6));
            Graphics graph1 = this.CreateGraphics();
            graph1.DrawRectangle(pen, new Rectangle(T.P, new Size(T.w, T.h)));
            if (isText) graph1.DrawString(T.Label, font, brushCl, new PointF(T.P.X + T.w / 2 - 30, T.P.Y + T.h / 2 - 6));
            //T.CPoint = T.getCPoint();
            //graph.DrawEllipse(pen   , T.CPoint.X - 5, T.CPoint.Y - 5, 10, 10);
            //graph1.DrawEllipse(pen, T.CPoint.X - 5, T.CPoint.Y - 5, 10, 10);
        }
        private void DrawTask(Task T, bool isText)
        {
            Pen pen = new Pen(brush, linewidth);

            Graphics graph = Pic.CreateGraphics();
            graph.DrawRectangle(pen, new Rectangle(T.P, new Size(T.w, T.h)));
            if (isText) graph.DrawString(T.Label, font, brush, new PointF(T.P.X +  10, T.P.Y + T.h / 2 - 6));
            Graphics graph1 = this.CreateGraphics();
            graph1.DrawRectangle(pen, new Rectangle(T.P, new Size(T.w, T.h)));
            if (isText) graph1.DrawString(T.Label, font, brush, new PointF(T.P.X + 10, T.P.Y + T.h / 2 - 6));
            T.getPoint();
            //T.CPoint = T.getCPoint();
            //graph.DrawEllipse(pen, T.CPoint.X - 5, T.CPoint.Y - 5, 10, 10);
            //graph1.DrawEllipse(pen, T.CPoint.X - 5, T.CPoint.Y - 5, 10, 10);
        }
        private void DrawTask(Task T, Color Cl)
        {

            Pen pen = new Pen(Cl, linewidth);

            Graphics graph = Pic.CreateGraphics();
            graph.DrawRectangle(pen, new Rectangle(T.P, new Size(T.w, T.h)));
            graph.DrawString(T.Label, font, brushSl, new PointF(T.P.X +10, T.P.Y + T.h / 2 - 6));
            Graphics graph1 = this.CreateGraphics();
            graph1.DrawRectangle(pen, new Rectangle(T.P, new Size(T.w, T.h)));
            graph1.DrawString(T.Label, font, brushSl, new PointF(T.P.X + 10, T.P.Y + T.h / 2 - 6));
            T.getPoint();
            graph.DrawRectangle(pen, new Rectangle(new Point(T.TPoint.X, T.TPoint.Y - 2), new Size(4, 4)));
            graph.DrawRectangle(pen, new Rectangle(new Point(T.BPoint.X, T.BPoint.Y - 2), new Size(4, 4)));
            graph.DrawRectangle(pen, new Rectangle(new Point(T.LPoint.X - 2, T.LPoint.Y), new Size(4, 4)));
            graph.DrawRectangle(pen, new Rectangle(new Point(T.RPoint.X - 2, T.RPoint.Y), new Size(4, 4)));
        }
        public new Graphics CreateGraphics()
        {
            return Graphics.FromImage(b);

        }
        private void this_Paint(object s, PaintEventArgs e)
        {
            DrawAll();
            //Graphics g = Pic.CreateGraphics();
           //g.DrawImage(b, 0, 0);

        }
        private void this_Resize(object s, EventArgs e)
        {
            DrawAll();
            //if (Pic.Width > 0 && Pic.Height > 0)
            //{

            //    Bitmap c = new Bitmap(Pic.Width, Pic.Height);
            //    Graphics g = Graphics.FromImage(c);
            //    g.DrawImage(b, 0, 0);
            //    b = c;
            //    g.Dispose();
            //}
        }

        private bool autoredraw;
        public bool AutoRedraw
        {
            get { return autoredraw; }
            set { autoredraw = value; }
        }
        void tTaskLabel_KeyUp(object sender, KeyEventArgs e)
        {
            
        }
        private void tbUpdateTask_Click(object sender, EventArgs e)
        {
            wF.lAction = lAction;
            wF.lTask = lTask;
            wF.lDeleteAction = lActionDelete;
            wF.lDeleteTask = lTaskDelete;
            if (wF.UpdataData())
            {
                MessageBox.Show("Update thành công");
                wF.GetWF(wF.tableID);
            }
            else
                MessageBox.Show("Có lỗi");
        }
        private void nTask_Click(object sender, EventArgs e)
        {
            SelectedTask = null;
            SelectedAction = null;

            state = State.Drawtask;
        }
        private void nAction_Click(object sender, EventArgs e)
        {
            SelectedTask = null;
            SelectedAction = null;
            state = State.DrawAction;
        }

        private void Ev_Load(object sender, EventArgs e)
        {
            DrawAll();
        }

        private void tTaskLabel_TextChanged(object sender, EventArgs e)
        {
            if (selectedTask != null )
            {
                ClearTask(selectedTask, true);
                selectedTask.Label = tTaskLabel.Text;
                DrawTask(selectedTask, Color.Red);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "<")
            {

                panel1.Width = 0;

                button1.Left = 0;
                button1.Text = ">";
            }
            else if (button1.Text == ">")
            {

                panel1.Width = 152;

                button1.Left = 152 - button1.Width;
                button1.Text = "<";
            }
        }

        private void tWFName_TextChanged(object sender, EventArgs e)
        {
            if (wF != null) wF.WFName = tWFName.Text;
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.AutoDo = cbAuto.Checked;
        }
        private void cbRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.isRefresh = cbRefresh.Checked;
        }
        private void tActionName_TextChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.Name = tActionName.Text;
        }

        private void tCondition_TextChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.Condition = tCondition.Text;
        }

        private void tbCommand_Click(object sender, EventArgs e)
        {
            if (selecteddAction != null)
            {
                fActionCommand fAcC = new fActionCommand();
                fAcC.Command = selecteddAction.Command;
                fAcC.AfterUpdateCommand = selecteddAction.AfterUpdateCommand;
                
                fAcC.ShowDialog();
                selecteddAction.Command = fAcC.Command;
                selecteddAction.AfterUpdateCommand = fAcC.AfterUpdateCommand;
            }
        }

        private void tbIcon_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Icon|*.Ico";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pIco.Image = Image.FromFile(dialog.FileName);
                if(selecteddAction !=null) selecteddAction.Icon=pIco.Image;
            }
        }

        private void tShowCond_TextChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.ShowCond = tShowCond.Text;
        }

        private void tbInputParameter_Click(object sender, EventArgs e)
        {
            fPara fpara = new fPara(selecteddAction);
            fpara.ShowDialog();
        }

        private void btSecurity_Click(object sender, EventArgs e)
        {
            if (selectedTask != null)
            {
                if (selectedTask.tbSecu == null)
                {
                    MessageBox.Show("You must update before set security");
                    return;
                }
                fUserTask fUT = new fUserTask(selectedTask.tbSecu, selectedTask.tbSecuGroup, selectedTask.id);
                fUT.ShowDialog();
                selectedTask.tbSecu = fUT.Data;
                selectedTask.tbSecuGroup = fUT.DataGr;
            }
        }

        private void tbTIcon_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Icon|*.Ico";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pTIcon.Image = Image.FromFile(dialog.FileName);
                if (selectedTask != null) selectedTask.Icon = pTIcon.Image;
            }
        }

        private void dbBegin_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTask != null)
            {
                selectedTask.isBegin = dbBegin.Checked;
                if (dbBegin.Checked)
                {
                    dbEnd.Checked = false;
                    dbCancel.Checked = false;
                }
            }
        }

        private void pIco_Click(object sender, EventArgs e)
        {

        }

        private void tApp_EditValueChanged(object sender, EventArgs e)
        {
            if (selectedTask != null) selectedTask.ApprovedStt = int.Parse(tApp.EditValue.ToString());
        }

        private void dbEnd_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTask != null)
            {
                selectedTask.isEnd = dbEnd.Checked;
                if (dbEnd.Checked)
                {
                    dbBegin.Checked = false;
                    dbCancel.Checked = false;
                }
            }
        }

        private void dbCancel_CheckedChanged(object sender, EventArgs e)
        {
            if (selectedTask != null)
            {
                selectedTask.isCancel = dbCancel.Checked;
                if (dbCancel.Checked)
                {
                    dbBegin.Checked = false;
                    dbEnd.Checked = false;
                }
            }
        }

        private void tConfirm_TextChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.Confirm = tConfirm.Text;
        }

        private void tMessage_TextChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.Message = tMessage.Text;
        }

        private void tbMail_Click(object sender, EventArgs e)
        {

            fActionMailContent fAcC = new fActionMailContent();
            fAcC.SendMail = selecteddAction.SendMail;
            fAcC.SendMailKH = selecteddAction.SendMailKH;
            fAcC.MailContent = selecteddAction.MailContent;
            fAcC.MailContentKH = selecteddAction.MailContentKH;
            fAcC.ShowDialog();
            selecteddAction.SendMail = fAcC.SendMail;
            selecteddAction.SendMailKH = fAcC.SendMailKH;
            selecteddAction.MailContent = fAcC.MailContent;
            selecteddAction.MailContentKH = fAcC.MailContentKH;
        }

        private void ckDoconfig_CheckedChanged(object sender, EventArgs e)
        {
            if (selecteddAction != null) selecteddAction.DoconfigData = ckDoconfig.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selecteddAction != null)
            {
                if (selecteddAction.tbSecu == null)
                {
                    MessageBox.Show("You must update before set security");
                    return;
                }
                fUserAction fUT = new fUserAction(selecteddAction.tbSecu, selecteddAction.tbSecuGroup, selecteddAction.Id);
                fUT.ShowDialog();
                selecteddAction.tbSecu = fUT.Data;
                selecteddAction.tbSecuGroup = fUT.DataGr;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}