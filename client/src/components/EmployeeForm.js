import React, { useState, useCallback } from 'react';
import { employeeService } from '../services/employeeService';
import './EmployeeForm.css';

export default function EmployeeForm({ onSuccess }) {
  const [formData, setFormData] = useState({
    name: '',
    department: '',
    salary: '',
  });
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const [validationErrors, setValidationErrors] = useState({});

  const validateForm = useCallback(() => {
    const errors = {};

    if (!formData.name.trim()) {
      errors.name = 'Name is required';
    } else if (formData.name.trim().length < 2) {
      errors.name = 'Name must be at least 2 characters';
    }

    if (!formData.department.trim()) {
      errors.department = 'Department is required';
    }

    if (!formData.salary) {
      errors.salary = 'Salary is required';
    } else if (isNaN(formData.salary) || formData.salary <= 0) {
      errors.salary = 'Salary must be a positive number';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  }, [formData]);

  const handleChange = useCallback((e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
    // Clear validation error for this field
    if (validationErrors[name]) {
      setValidationErrors(prev => ({
        ...prev,
        [name]: '',
      }));
    }
  }, [validationErrors]);

  const handleSubmit = useCallback(async (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      setError(null);

      await employeeService.create({
        name: formData.name.trim(),
        department: formData.department.trim(),
        salary: parseFloat(formData.salary),
      });

      setFormData({ name: '', department: '', salary: '' });
      setValidationErrors({});
      onSuccess?.();
    } catch (err) {
      setError(err.message || 'Failed to create employee');
      console.error('Create employee error:', err);
    } finally {
      setLoading(false);
    }
  }, [formData, validateForm, onSuccess]);

  return (
    <div className="form-container">
      <h2>Add New Employee</h2>

      {error && <div className="error-message">{error}</div>}

      <form onSubmit={handleSubmit} className="employee-form">
        <div className="form-group">
          <label htmlFor="name">Name *</label>
          <input
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            placeholder="Enter employee name"
            className={validationErrors.name ? 'input-error' : ''}
          />
          {validationErrors.name && (
            <span className="error-text">{validationErrors.name}</span>
          )}
        </div>

        <div className="form-group">
          <label htmlFor="department">Department *</label>
          <input
            type="text"
            id="department"
            name="department"
            value={formData.department}
            onChange={handleChange}
            placeholder="Enter department"
            className={validationErrors.department ? 'input-error' : ''}
          />
          {validationErrors.department && (
            <span className="error-text">{validationErrors.department}</span>
          )}
        </div>

        <div className="form-group">
          <label htmlFor="salary">Salary *</label>
          <input
            type="number"
            id="salary"
            name="salary"
            value={formData.salary}
            onChange={handleChange}
            placeholder="Enter salary"
            step="0.01"
            min="0"
            className={validationErrors.salary ? 'input-error' : ''}
          />
          {validationErrors.salary && (
            <span className="error-text">{validationErrors.salary}</span>
          )}
        </div>

        <div className="form-actions">
          <button
            type="submit"
            className="btn btn-success"
            disabled={loading}
          >
            {loading ? 'Creating...' : 'Create Employee'}
          </button>
        </div>
      </form>
    </div>
  );
}
