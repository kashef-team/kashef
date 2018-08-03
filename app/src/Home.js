import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";

import TopAppBar from './TopAppBar';
import BottomBar from './BottomBar';

const styles = {
  bottom: {
    position: 'fixed',
    bottom: 0,
  }
};

class Home extends Component {
    render() {
      const { classes } = this.props;

      return (
        <div>
            <TopAppBar />
            
            <BottomBar />
        </div>
      );
    }
  }
  
  export default withStyles(styles)(Home);