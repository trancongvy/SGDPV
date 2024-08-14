namespace DevControls
{
    partial class CDTGridLookUpEdit
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fProperties = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            this.fPropertiesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fPropertiesView)).BeginInit();
            this.SuspendLayout();
            // 
            // fProperties
            // 
            this.fProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.fProperties.Name = "fProperties";
            this.fProperties.View = this.fPropertiesView;
            // 
            // fPropertiesView
            // 
            this.fPropertiesView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.fPropertiesView.Name = "fPropertiesView";
            this.fPropertiesView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.fPropertiesView.OptionsView.ShowGroupPanel = false;
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fPropertiesView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit fProperties;
        private DevExpress.XtraGrid.Views.Grid.GridView fPropertiesView;
    }
}
