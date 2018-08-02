import React, { Component } from 'react';
import { Admin, Resource } from 'react-admin';

import restProvider from './sails-rest-provider';
import logo from './logo.svg';
import './App.css';

import { PersonList } from './persons';

const App = () => (
  <Admin dataProvider={restProvider('http://localhost:1337')}>
    <Resource name="person" list={PersonList} />
  </Admin>
);

export default App;
