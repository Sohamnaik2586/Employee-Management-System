const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5037/api';

const request = async (endpoint, options = {}) => {
  const url = `${API_BASE_URL}${endpoint}`;

  try {
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    });

    if (!response.ok) {
      let errorMessage = `HTTP Error: ${response.status}`;
      try {
        const error = await response.json();
        errorMessage = error.message || error.error || errorMessage;
      } catch {
        errorMessage = response.statusText || errorMessage;
      }
      throw new Error(errorMessage);
    }

    const contentType = response.headers.get('content-type');
    return contentType?.includes('application/json') ? await response.json() : null;
  } catch (error) {
    if (error instanceof TypeError && error.message.includes('Failed to fetch')) {
      throw new Error(`Cannot connect to API at ${API_BASE_URL}. Ensure the backend is running.`);
    }
    throw error;
  }
};

export const employeeService = {
  getAll: () => request('/employees', { method: 'GET' }),
  getById: (id) => request(`/employees/${id}`, { method: 'GET' }),
  create: (data) => request('/employees', { method: 'POST', body: JSON.stringify(data) }),
  update: (id, data) => request(`/employees/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id) => request(`/employees/${id}`, { method: 'DELETE' }),
};
