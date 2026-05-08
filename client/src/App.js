import React, { useState, useCallback } from 'react';
import './App.css';
import EmployeeList from './components/EmployeeList';
import EmployeeForm from './components/EmployeeForm';
import Footer from './components/Footer';

function App() {
  const [showForm, setShowForm] = useState(false);
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleEmployeeCreated = useCallback(() => {
    setRefreshTrigger(prev => prev + 1);
    setShowForm(false);
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <h1>Employee Management System</h1>
        <p>Manage your employee database efficiently</p>
      </header>
      <main className="App-main">
        <div className="header-controls">
          <button 
            className="btn btn-primary" 
            onClick={() => setShowForm(!showForm)}
          >
            {showForm ? 'Cancel' : '+ Add New Employee'}
          </button>
        </div>

        <div className={`form-wrapper ${showForm ? 'show' : ''}`}>
          {showForm && <EmployeeForm onSuccess={handleEmployeeCreated} />}
        </div>

        <EmployeeList refreshTrigger={refreshTrigger} onEmployeeDeleted={handleEmployeeCreated} />
      </main>
      <Footer />
    </div>
  );
}

export default App;
