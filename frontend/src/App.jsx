// src/App.jsx
import { useState, useEffect } from 'react';
import api from './services/api';
import './App.css';

function App() {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Función para llamar a tu backend en .NET
    const fetchData = async () => {
      try {
        // Reemplaza '/endpoint' con la ruta real de tu controlador (ej. '/users' o '/products')
        const response = await api.get('/endpoint'); 
        setData(response.data);
      } catch (err) {
        setError('Error al conectar con la API');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  return (
    <div className="container">
      <h1>Mi Proyecto REST .NET + React</h1>
      
      {loading && <p>Cargando datos...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      
      <ul>
        {data.map((item, index) => (
          // Ajusta 'item.name' o 'item.id' según la estructura del JSON que devuelve tu API
          <li key={index}>{JSON.stringify(item)}</li> 
        ))}
      </ul>
    </div>
  );
}

export default App;