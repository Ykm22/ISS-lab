using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using model;

namespace client
{
    public partial class PharmacyWindow : Form
    {
        private Form loginWindow;
        private PharmacyController ctrl;
        private IList<Medicine> modelMedicines;
        public void setLoginWindow(Form loginWindow)
        {
            this.loginWindow = loginWindow;
        }
        public PharmacyWindow(PharmacyController ctrl)
        {
            InitializeComponent();
            this.ctrl = ctrl;
            modelMedicines = ctrl.GetAllMedicines();
            dataGridView_Medicines.DataSource = modelMedicines;
            ctrl.updateEvent += userUpdate;
        }
        private void userUpdate(object Sender, UserEventArgs E)
        {
            if (E.UserEventType == UserEvent.Update_AddedMedicine)
            {
                Medicine addedMedicine = (Medicine)E.Data;
                modelMedicines.Add(addedMedicine);
                dataGridView_Medicines.DataSource = null;
                dataGridView_Medicines.DataSource = modelMedicines;
            }

            if (E.UserEventType == UserEvent.Update_UpdatedMedicine)
            {
                Medicine updatedMedicine = (Medicine)E.Data;
                Medicine toUpdateMedicine = modelMedicines.FirstOrDefault(m => m.Id == updatedMedicine.Id);
                if (toUpdateMedicine != null)
                {
                    toUpdateMedicine.Name = updatedMedicine.Name;
                    toUpdateMedicine.Purpose = updatedMedicine.Purpose;
                    toUpdateMedicine.AvailableQuantity = updatedMedicine.AvailableQuantity;
                }

                dataGridView_Medicines.DataSource = null;
                dataGridView_Medicines.DataSource = modelMedicines;
            }

            if (E.UserEventType == UserEvent.Update_DeletedMedicine)
            {
                Medicine medicine = (Medicine)E.Data;
                int IdToDelete = medicine.Id;
                Medicine toDeleteMedicine = modelMedicines.FirstOrDefault(m => m.Id == IdToDelete);
                if (toDeleteMedicine != null)
                {
                    modelMedicines.Remove(toDeleteMedicine);
                }

                dataGridView_Medicines.DataSource = null;
                dataGridView_Medicines.DataSource = modelMedicines;
            }
        }
        private void button_AddMedicine_Click(object sender, EventArgs e)
        {
            AddMedicineWindow addMedicineWindow = new AddMedicineWindow(ctrl);
            addMedicineWindow.Show();
        }

        private void button_UpdateMedicine_Click(object sender, EventArgs e)
        {
            UpdateMedicineWindow updateMedicineWindow = new UpdateMedicineWindow(ctrl);
            updateMedicineWindow.Show();
        }

        private void button_DeleteMedicine_Click(object sender, EventArgs e)
        {
            DeleteMedicineWindow deleteMedicineWindow = new DeleteMedicineWindow(ctrl);
            deleteMedicineWindow.Show();
        }

        private void button_FilterMedicines_Click(object sender, EventArgs e)
        {
            FilterMedicinesWindow filterMedicinesWindow = new FilterMedicinesWindow(ctrl, dataGridView_Medicines);
            filterMedicinesWindow.Show();
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            modelMedicines = ctrl.GetAllMedicines();
            dataGridView_Medicines.DataSource = null;
            dataGridView_Medicines.DataSource = modelMedicines;
        }
    }
}