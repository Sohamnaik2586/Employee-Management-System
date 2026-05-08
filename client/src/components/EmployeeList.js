import React, { useState, useEffect, useCallback } from 'react';
import { employeeService } from '../services/employeeService';
import UpdateEmployeeForm from './UpdateEmployeeForm';
import ConfirmDialog from './ConfirmDialog';
import './EmployeeList.css';

export default function EmployeeList({ refreshTrigger, onEmployeeDeleted }) {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortField, setSortField] = useState('name');
  const [sortOrder, setSortOrder] = useState('asc');
  const [editingId, setEditingId] = useState(null);
  const [deleteConfirm, setDeleteConfirm] = useState({ isOpen: false, id: null, name: '' });

  const fetchEmployees = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await employeeService.getAll();
      setEmployees(response.data || []);
    } catch (err) {
      setError(err.message);
      setEmployees([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchEmployees();
  }, [fetchEmployees, refreshTrigger]);

  const handleDeleteClick = useCallback((id, name) => {
    setDeleteConfirm({ isOpen: true, id, name });
  }, []);

  const handleConfirmDelete = useCallback(async () => {
    try {
      await employeeService.delete(deleteConfirm.id);
      setDeleteConfirm({ isOpen: false, id: null, name: '' });
      fetchEmployees();
      onEmployeeDeleted?.();
    } catch (err) {
      setError(`Failed to delete employee: ${err.message}`);
      setDeleteConfirm({ isOpen: false, id: null, name: '' });
    }
  }, [deleteConfirm.id, fetchEmployees, onEmployeeDeleted]);

  const handleCancelDelete = useCallback(() => {
    setDeleteConfirm({ isOpen: false, id: null, name: '' });
  }, []);

  const handleSort = useCallback((field) => {
    if (sortField === field) {
      setSortOrder(prev => prev === 'asc' ? 'desc' : 'asc');
    } else {
      setSortField(field);
      setSortOrder('asc');
    }
  }, [sortField]);

  const getSortIndicator = (field) => {
    if (sortField !== field) return '';
    return sortOrder === 'asc' ? ' [ASC]' : ' [DESC]';
  };

  const filteredAndSortedEmployees = employees
    .filter(emp => {
      const searchLower = searchTerm.toLowerCase();
      return (
        emp.name.toLowerCase().includes(searchLower) ||
        emp.department.toLowerCase().includes(searchLower)
      );
    })
    .sort((a, b) => {
      let aValue = a[sortField];
      let bValue = b[sortField];

      if (typeof aValue === 'string') {
        aValue = aValue.toLowerCase();
        bValue = bValue.toLowerCase();
      }

      return sortOrder === 'asc' ? (aValue > bValue ? 1 : -1) : (aValue < bValue ? 1 : -1);
    });

  if (loading) {
    return <div className="loading">Loading employees...</div>;
  }

  const editingEmployee = editingId ? employees.find(emp => emp.id === editingId) : null;

  return (
    <div className="employee-list-container">
      <ConfirmDialog
        isOpen={deleteConfirm.isOpen}
        title="Delete Employee"
        message={`Are you sure you want to delete ${deleteConfirm.name}? This action cannot be undone.`}
        confirmText="Delete"
        cancelText="Cancel"
        isDangerous={true}
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
      />

      {error && (
        <div className="error-message">
          <div>{error}</div>
          <button className="error-retry-btn" onClick={fetchEmployees}>
            Retry
          </button>
        </div>
      )}

      {editingEmployee ? (
        <UpdateEmployeeForm
          employee={editingEmployee}
          onSuccess={() => {
            fetchEmployees();
            setEditingId(null);
          }}
          onCancel={() => setEditingId(null)}
        />
      ) : (
        <>
          <div className="controls">
            <div className="search-box">
              <input
                type="text"
                placeholder="Search by name or department..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="search-input"
              />
            </div>
            <button className="btn btn-secondary" onClick={fetchEmployees}>
              Refresh
            </button>
          </div>

          {filteredAndSortedEmployees.length === 0 ? (
            <div className="empty-state">
              <p>No employees found</p>
            </div>
          ) : (
            <div className="table-wrapper">
              <table className="employee-table">
                <thead>
                  <tr>
                    <th onClick={() => handleSort('id')}>
                      ID<span className="sort-indicator">{getSortIndicator('id')}</span>
                    </th>
                    <th onClick={() => handleSort('name')}>
                      Name<span className="sort-indicator">{getSortIndicator('name')}</span>
                    </th>
                    <th onClick={() => handleSort('department')}>
                      Department<span className="sort-indicator">{getSortIndicator('department')}</span>
                    </th>
                    <th onClick={() => handleSort('salary')}>
                      Salary<span className="sort-indicator">{getSortIndicator('salary')}</span>
                    </th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredAndSortedEmployees.map((emp) => (
                    <tr key={emp.id} className="employee-row">
                      <td>{emp.id}</td>
                      <td>{emp.name}</td>
                      <td>{emp.department}</td>
                      <td>${emp.salary.toFixed(2)}</td>
                      <td className="actions">
                        <button
                          className="btn btn-primary btn-sm"
                          onClick={() => setEditingId(emp.id)}
                        >
                          Edit
                        </button>
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => handleDeleteClick(emp.id, emp.name)}
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          <div className="employee-count">
            Total: {filteredAndSortedEmployees.length} employee(s)
          </div>
        </>
      )}
    </div>
  );
}

