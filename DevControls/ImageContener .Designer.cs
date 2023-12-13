namespace DevControls
{
    partial class ImageContener
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
            this.fProperties = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // fProperties
            // 
            this.fProperties.Name = "fProperties";
            // 
            // ImageContener
            // 
            this.Name = "fileContener";
            this.Size = new System.Drawing.Size(579, 23);
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit fProperties;
    }
}
